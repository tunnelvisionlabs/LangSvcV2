namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing4;
    using ParseResultEventArgs = Tvl.VisualStudio.Language.Parsing.ParseResultEventArgs;

    internal sealed class Antlr4OutliningTagger : ITagger<IOutliningRegionTag>
    {
        private readonly Antlr4OutliningTaggerProvider _provider;
        private readonly ITextBuffer _textBuffer;
        private readonly Antlr4BackgroundParser _backgroundParser;

        private List<ITagSpan<IOutliningRegionTag>> _outliningRegions;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public Antlr4OutliningTagger(Antlr4OutliningTaggerProvider provider, ITextBuffer textBuffer)
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");

            _provider = provider;
            _textBuffer = textBuffer;

            _backgroundParser = (Antlr4BackgroundParser)provider.BackgroundParserFactoryService.GetBackgroundParser(textBuffer);
            _backgroundParser.ParseComplete += HandleBackgroundParseComplete;
            _backgroundParser.RequestParse(false);
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _outliningRegions ?? Enumerable.Empty<ITagSpan<IOutliningRegionTag>>();
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }

        private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
            if (antlrParseResultArgs == null)
                return;

            UpdateTags(antlrParseResultArgs);
        }

        private void UpdateTags(AntlrParseResultEventArgs antlrParseResultArgs)
        {
            Contract.Requires<ArgumentNullException>(antlrParseResultArgs != null, "antlrParseResultArgs");

            OutliningRegionListener listener = new OutliningRegionListener(antlrParseResultArgs.Snapshot, antlrParseResultArgs.Tokens);
            ParseTreeWalker.Default.Walk(listener, antlrParseResultArgs.Result);
            _outliningRegions = listener.OutliningRegions;
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(antlrParseResultArgs.Snapshot, new Span(0, antlrParseResultArgs.Snapshot.Length))));
        }

        private class OutliningRegionListener : GrammarParserBaseListener
        {
            private readonly ITextSnapshot _snapshot;
            private readonly IList<IToken> _tokens;
            private readonly List<ITagSpan<IOutliningRegionTag>> _outliningRegions = new List<ITagSpan<IOutliningRegionTag>>();

            private int _tokenIndexLimit = -1;

            public OutliningRegionListener(ITextSnapshot snapshot, IList<IToken> tokens)
            {
                Contract.Requires<ArgumentNullException>(snapshot != null, "snapshot");
                Contract.Requires<ArgumentNullException>(tokens != null, "tokens");

                _snapshot = snapshot;
                _tokens = tokens;
            }

            public List<ITagSpan<IOutliningRegionTag>> OutliningRegions
            {
                get
                {
                    Contract.Ensures(Contract.Result<List<ITagSpan<IOutliningRegionTag>>>() != null);
                    return _outliningRegions;
                }
            }

            public override void EnterEveryRule([NotNull]ParserRuleContext context)
            {
                Interval sourceInterval = context.SourceInterval;
                if (sourceInterval.a < 0 || sourceInterval.b < 0 || sourceInterval.Length <= 0)
                    return;

                Stack<Tuple<IToken, string>> multilineTokens = null;

                int lowerBound = _tokenIndexLimit + 1;
                for (int i = sourceInterval.a - 1; i >= lowerBound; i--)
                {
                    IToken token = _tokens[i];
                    if (token.Channel == TokenConstants.DefaultChannel)
                        break;

                    string hint = null;
                    if (token.Type == GrammarParser.DOC_COMMENT)
                        hint = "/** ... */";
                    else if (token.Type == GrammarParser.BLOCK_COMMENT)
                        hint = "/* ... */";

                    if (hint != null)
                    {
                        if (multilineTokens == null)
                            multilineTokens = new Stack<Tuple<IToken, string>>();

                        multilineTokens.Push(Tuple.Create(token, hint));
                    }
                }

                if (multilineTokens != null)
                {
                    while (multilineTokens.Count > 0)
                    {
                        Tuple<IToken, string> multilineToken = multilineTokens.Pop();
                        OutlineToken(multilineToken.Item1, multilineToken.Item2);
                    }
                }

                _tokenIndexLimit = sourceInterval.a;
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_tokensSpec, 1, Dependents.Self)]
            public override void EnterTokensSpec([NotNull]AbstractGrammarParser.TokensSpecContext context)
            {
                OutlineBlock(context, "tokens { ... }");
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_channelsSpec, 6, Dependents.Self)]
            public override void EnterChannelsSpec([NotNull]AbstractGrammarParser.ChannelsSpecContext context)
            {
                OutlineBlock(context, "channels { ... }");
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_optionsSpec, 3, Dependents.Self)]
            public override void EnterOptionsSpec([NotNull]AbstractGrammarParser.OptionsSpecContext context)
            {
                OutlineBlock(context, "options { ... }");
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_action, 0, Dependents.Self)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_actionScopeName, 6, Dependents.Self | Dependents.Descendants)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_id, 1, Dependents.Self | Dependents.Descendants)]
            public override void EnterAction([NotNull]AbstractGrammarParser.ActionContext context)
            {
                string hint = "@";
                if (context.actionScopeName() != null)
                    hint += context.actionScopeName().GetText() + "::";

                if (context.id() != null)
                    hint += context.id().GetText();
                else
                    hint += "?";

                hint += " { ... }";

                // these are just the prequel construct
                OutlineBlock(context, hint);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_parserRuleSpec, 0, Dependents.Self)]
            public override void EnterParserRuleSpec([NotNull]AbstractGrammarParser.ParserRuleSpecContext context)
            {
                string hint;
                if (context.RULE_REF() != null)
                    hint = context.RULE_REF().GetText() + "...";
                else
                    hint = "...";

                OutlineBlock(context, hint);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerRule, 0, Dependents.Self)]
            public override void EnterLexerRule([NotNull]AbstractGrammarParser.LexerRuleContext context)
            {
                string hint;
                if (context.TOKEN_REF() != null)
                    hint = context.TOKEN_REF().GetText() + "...";
                else
                    hint = "...";

                OutlineBlock(context, hint);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_modeSpec, 3, Dependents.Self)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_id, 1, Dependents.Self | Dependents.Descendants)]
            public override void EnterModeSpec([NotNull]AbstractGrammarParser.ModeSpecContext context)
            {
                string hint;
                if (context.id() != null)
                    hint = "mode " + context.id().GetText() + "...";
                else
                    hint = "mode...";

                OutlineBlock(context, hint);
            }

            private void OutlineBlock(IParseTree context, object collapsedForm)
            {
                Interval sourceInterval = context.SourceInterval;
                if (sourceInterval.a < 0 || sourceInterval.b < 0 || sourceInterval.Length <= 0)
                    return;

                IToken startToken = _tokens[sourceInterval.a];
                IToken stopToken = _tokens[sourceInterval.b];
                OutlineBlock(startToken, stopToken, collapsedForm);
            }

            private void OutlineToken(IToken context, object collapsedForm)
            {
                OutlineBlock(context, context, collapsedForm);
            }

            private void OutlineBlock(IToken startToken, IToken stopToken, object collapsedForm)
            {
                Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);

                // don't collapse blocks that don't span multiple lines
                if (_snapshot.GetLineNumberFromPosition(span.Start) == _snapshot.GetLineNumberFromPosition(span.End))
                    return;

                SnapshotSpan snapshotSpan = new SnapshotSpan(_snapshot, span);
                IOutliningRegionTag tag = new OutliningRegionTag(false, false, collapsedForm, snapshotSpan.GetText());
                TagSpan<IOutliningRegionTag> tagSpan = new TagSpan<IOutliningRegionTag>(snapshotSpan, tag);
                _outliningRegions.Add(tagSpan);
            }
        }
    }
}
