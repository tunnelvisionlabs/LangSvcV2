namespace Tvl.VisualStudio.Language.Alloy.Experimental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter;
    using Tvl.VisualStudio.Shell.OutputWindow;

    internal sealed class AlloyAtnOutliningTagger : BackgroundParser, ITagger<IOutliningRegionTag>
    {
        private List<ITagSpan<IOutliningRegionTag>> _outliningRegions;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public AlloyAtnOutliningTagger(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            RequestParse(false);
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _outliningRegions ?? Enumerable.Empty<ITagSpan<IOutliningRegionTag>>();
        }

        protected override void ReParseImpl()
        {
            // lex the entire document to get the set of identifiers we'll need to classify
            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;
            var input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            var lexer = new AlloyLexer(input);
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();

            /* Want to outline the following blocks:
             *  - assert
             *  - enum
             *  - fact
             *  - fun
             *  - pred
             *  - sig (block and body)
             */

            List<IToken> outliningKeywords = new List<IToken>();
            while (tokens.LA(1) != CharStreamConstants.EndOfFile)
            {
                switch (tokens.LA(1))
                {
                case AlloyLexer.KW_ASSERT:
                case AlloyLexer.KW_ENUM:
                case AlloyLexer.KW_FACT:
                case AlloyLexer.KW_FUN:
                case AlloyLexer.KW_PRED:
                case AlloyLexer.KW_SIG:
                    outliningKeywords.Add(tokens.LT(1));
                    break;

                case CharStreamConstants.EndOfFile:
                    goto doneLexing;

                default:
                    break;
                }

                tokens.Consume();
            }

        doneLexing:

            List<ITagSpan<IOutliningRegionTag>> outliningRegions = new List<ITagSpan<IOutliningRegionTag>>();
            foreach (var token in outliningKeywords)
            {
                tokens.Seek(token.TokenIndex);
                tokens.Consume();

                NetworkInterpreter interpreter = CreateNetworkInterpreter(tokens);
                while (interpreter.TryStepBackward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedStart))
                        break;
                }

                interpreter.CombineBoundedStartContexts();

                while (interpreter.TryStepForward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedEnd))
                        break;
                }

                foreach (var context in interpreter.Contexts)
                {
                    InterpretTraceTransition firstBraceTransition = context.Transitions.FirstOrDefault(i => i.Symbol == AlloyLexer.LBRACE);
                    InterpretTraceTransition lastBraceTransition = context.Transitions.LastOrDefault(i => i.Transition.IsMatch);
                    if (firstBraceTransition == null || lastBraceTransition == null)
                        continue;

                    if (token.Type == AlloyLexer.KW_SIG)
                    {
                        InterpretTraceTransition lastBodyBraceTransition = context.Transitions.LastOrDefault(i => i.Symbol == AlloyLexer.RBRACE && interpreter.Network.StateRules[i.Transition.SourceState.Id].Name == AlloyOutliningAtnBuilder.RuleNames.SigBody);
                        if (lastBodyBraceTransition != lastBraceTransition)
                        {
                            var bodySpan = OutlineBlock(firstBraceTransition.Token, lastBodyBraceTransition.Token, snapshot);
                            if (bodySpan != null)
                                outliningRegions.Add(bodySpan);

                            firstBraceTransition = context.Transitions.LastOrDefault(i => i.Symbol == AlloyLexer.LBRACE && i.TokenIndex > lastBodyBraceTransition.TokenIndex);
                        }
                    }

                    var blockSpan = OutlineBlock(firstBraceTransition.Token, lastBraceTransition.Token, snapshot);
                    if (blockSpan != null)
                        outliningRegions.Add(blockSpan);
                }
            }

            _outliningRegions = outliningRegions;
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, new Span(0, snapshot.Length))));
        }

        private ITagSpan<IOutliningRegionTag> OutlineBlock(IToken firstBrace, IToken lastBrace, ITextSnapshot snapshot)
        {
            Span span = Span.FromBounds(firstBrace.StartIndex, lastBrace.StopIndex + 1);
            if (snapshot.GetLineNumberFromPosition(span.Start) == snapshot.GetLineNumberFromPosition(span.End))
                return null;

            SnapshotSpan snapshotSpan = new SnapshotSpan(snapshot, span);
            IOutliningRegionTag tag = new OutliningRegionTag("...", "...");
            TagSpan<IOutliningRegionTag> tagSpan = new TagSpan<IOutliningRegionTag>(snapshotSpan, tag);
            return tagSpan;
        }

        private NetworkInterpreter CreateNetworkInterpreter(ITokenStream tokens)
        {
            Network network = NetworkBuilder<AlloyOutliningAtnBuilder>.GetOrBuildNetwork();

            NetworkInterpreter interpreter = new NetworkInterpreter(network, tokens);

            //RuleBinding memberSelectRule = network.GetRule(AlloyOutliningAtnBuilder.RuleNames.BinOpExpr18);
            //interpreter.BoundaryRules.Add(memberSelectRule);

            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.LetDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.QuantDecls));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Decl));
            //////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.NameList));
            //////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.NameListName));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Ref));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Open));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FactDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.AssertDecl));
            //////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FunDecl));
            //////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FunctionName));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.CmdDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Typescope));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.EnumDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.ElseClause));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Module));

            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.LetDecl));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.QuantDecls));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Decl));

            // /* adding this rule definitely didn't help! */
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Expr));

            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Module));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FactDeclHeader));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.AssertDeclHeader));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FunFunctionName));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.PredFunctionName));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FunctionReturn));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.SigDeclHeader));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.SigExt));

            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.AssertDecl));
            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.EnumDecl));
            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FactDecl));
            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FunDecl));
            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.SigDecl));

            ////interpreter.ExcludedStartRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.CallArguments));
            interpreter.ExcludedStartRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.SigBody));
            interpreter.ExcludedStartRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Block));

            return interpreter;
        }

        //private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
        //{
        //    AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
        //    if (antlrParseResultArgs == null)
        //        return;

        //    UpdateTags(antlrParseResultArgs);
        //}

        //private void UpdateTags(AntlrParseResultEventArgs antlrParseResultArgs)
        //{
        //    List<ITagSpan<IOutliningRegionTag>> outliningRegions = null;
        //    IAstRuleReturnScope parseResult = antlrParseResultArgs.Result as IAstRuleReturnScope;
        //    if (parseResult != null)
        //        outliningRegions = AlloyOutliningTaggerWalker.ExtractOutliningRegions(parseResult, antlrParseResultArgs.Tokens, _provider, antlrParseResultArgs.Snapshot);

        //    this._outliningRegions = outliningRegions ?? new List<ITagSpan<IOutliningRegionTag>>();
        //    OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(antlrParseResultArgs.Snapshot, new Span(0, antlrParseResultArgs.Snapshot.Length))));
        //}

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
