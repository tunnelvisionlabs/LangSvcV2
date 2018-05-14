namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing4;
    using Tvl.VisualStudio.Text.Navigation;
    using AntlrEditorNavigationTypeNames = Tvl.VisualStudio.Language.Antlr3.AntlrEditorNavigationTypeNames;
    using ParseResultEventArgs = Tvl.VisualStudio.Language.Parsing.ParseResultEventArgs;

    internal sealed class Antlr4EditorNavigationSource : IEditorNavigationSource
    {
        private readonly Antlr4EditorNavigationSourceProvider _provider;
        private readonly ITextBuffer _textBuffer;
        private readonly Antlr4BackgroundParser _backgroundParser;

        private readonly IEditorNavigationType _parserRuleNavigationType;
        private readonly IEditorNavigationType _lexerRuleNavigationType;
        private readonly ImageSource _lexerRuleGlyph;
        private readonly ImageSource _parserRuleGlyph;

        private List<IEditorNavigationTarget> _navigationTargets;

        public event EventHandler NavigationTargetsChanged;

        public Antlr4EditorNavigationSource([NotNull] Antlr4EditorNavigationSourceProvider provider, [NotNull] ITextBuffer textBuffer)
        {
            Requires.NotNull(provider, nameof(provider));
            Requires.NotNull(textBuffer, nameof(textBuffer));

            _provider = provider;
            _textBuffer = textBuffer;

            _parserRuleNavigationType = provider.EditorNavigationTypeRegistryService.GetEditorNavigationType(AntlrEditorNavigationTypeNames.ParserRule);
            _lexerRuleNavigationType = provider.EditorNavigationTypeRegistryService.GetEditorNavigationType(AntlrEditorNavigationTypeNames.LexerRule);

            string assemblyName = typeof(Antlr4EditorNavigationSource).Assembly.GetName().Name;
            _lexerRuleGlyph = new BitmapImage(new Uri(string.Format("pack://application:,,,/{0};component/Resources/lexericon.png", assemblyName)));
            _parserRuleGlyph = new BitmapImage(new Uri(string.Format("pack://application:,,,/{0};component/Resources/parsericon.png", assemblyName)));

            _backgroundParser = (Antlr4BackgroundParser)provider.BackgroundParserFactoryService.GetBackgroundParser(textBuffer);
            _backgroundParser.ParseComplete += HandleBackgroundParseComplete;
            _backgroundParser.RequestParse(false);
        }

        public IEnumerable<IEditorNavigationType> GetNavigationTypes()
        {
            yield return _parserRuleNavigationType;
            yield return _lexerRuleNavigationType;
        }

        public IEnumerable<IEditorNavigationTarget> GetNavigationTargets()
        {
            return _navigationTargets ?? Enumerable.Empty<IEditorNavigationTarget>();
        }

        private void OnNavigationTargetsChanged(EventArgs e)
        {
            var t = NavigationTargetsChanged;
            if (t != null)
                t(this, e);
        }

        private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
            if (antlrParseResultArgs == null)
                return;

            UpdateNavigationTargets(antlrParseResultArgs);
        }

        private void UpdateNavigationTargets([NotNull] AntlrParseResultEventArgs antlrParseResultArgs)
        {
            Requires.NotNull(antlrParseResultArgs, nameof(antlrParseResultArgs));

            NavigationTargetListener listener = new NavigationTargetListener(this, antlrParseResultArgs.Snapshot, antlrParseResultArgs.Tokens);
            ParseTreeWalker.Default.Walk(listener, antlrParseResultArgs.Result);
            _navigationTargets = listener.NavigationTargets;
            OnNavigationTargetsChanged(EventArgs.Empty);
        }

        private class NavigationTargetListener : GrammarParserBaseListener
        {
            private readonly Antlr4EditorNavigationSource _navigationSource;
            private readonly ITextSnapshot _snapshot;
            private readonly IList<IToken> _tokens;
            private readonly List<IEditorNavigationTarget> _navigationTargets = new List<IEditorNavigationTarget>();

            private readonly Stack<string> _mode = new Stack<string>();

            public NavigationTargetListener([NotNull] Antlr4EditorNavigationSource navigationSource, [NotNull] ITextSnapshot snapshot, [NotNull] IList<IToken> tokens)
            {
                Requires.NotNull(navigationSource, nameof(navigationSource));
                Requires.NotNull(snapshot, nameof(snapshot));
                Requires.NotNull(tokens, nameof(tokens));

                _navigationSource = navigationSource;
                _snapshot = snapshot;
                _tokens = tokens;
            }

            [NotNull]
            public List<IEditorNavigationTarget> NavigationTargets
            {
                get
                {
                    return _navigationTargets;
                }
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_parserRuleSpec, 3, Dependents.Self | Dependents.Ancestors)]
            public override void EnterParserRuleSpec([NotNull]AbstractGrammarParser.ParserRuleSpecContext context)
            {
                AddNavigationTarget(context, context.RULE_REF(), _navigationSource._parserRuleNavigationType, _navigationSource._parserRuleGlyph);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_lexerRule, 3, Dependents.Self | Dependents.Ancestors)]
            public override void EnterLexerRule([NotNull]AbstractGrammarParser.LexerRuleContext context)
            {
                AddNavigationTarget(context, context.TOKEN_REF(), _navigationSource._lexerRuleNavigationType, _navigationSource._lexerRuleGlyph);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_tokensSpec, 6, Dependents.Self | Dependents.Ancestors)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_id, 1, Dependents.Descendants)]
            public override void EnterTokensSpec([NotNull]AbstractGrammarParser.TokensSpecContext context)
            {
                foreach (GrammarParser.IdContext id in context.id())
                    AddNavigationTarget(id, ParseTrees.GetStartNode(id), _navigationSource._lexerRuleNavigationType, _navigationSource._lexerRuleGlyph);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_channelsSpec, 6, Dependents.Self)]
            public override void EnterChannelsSpec([NotNull]AbstractGrammarParser.ChannelsSpecContext context)
            {
                // should we do anything here?
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_modeSpec, 3, Dependents.Self | Dependents.Ancestors)]
            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_id, 1, Dependents.Descendants)]
            public override void EnterModeSpec([NotNull]AbstractGrammarParser.ModeSpecContext context)
            {
                string modeName = context.id() != null ? context.id().GetText() : null;
                if (string.IsNullOrEmpty(modeName))
                    modeName = "?";

                _mode.Push(modeName);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_modeSpec, 3, Dependents.Self | Dependents.Ancestors)]
            public override void ExitModeSpec([NotNull]AbstractGrammarParser.ModeSpecContext context)
            {
                _mode.Pop();
            }

            private void AddNavigationTarget([NotNull] IParseTree tree, ITerminalNode identifier, [NotNull] IEditorNavigationType navigationType, ImageSource glyph)
            {
                Debug.Assert(tree != null);
                Debug.Assert(navigationType != null);

                Interval sourceInterval = tree.SourceInterval;
                if (sourceInterval.a < 0 || sourceInterval.b < 0 || sourceInterval.Length <= 0)
                    return;

                IToken startToken = _tokens[sourceInterval.a];
                IToken stopToken = _tokens[sourceInterval.b];
                Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                Span seek;
                string name = identifier != null ? identifier.GetText() : null;
                if (string.IsNullOrEmpty(name))
                {
                    seek = new Span(span.Start, 0);
                    name = "?";
                }
                else
                {
                    seek = new Span(identifier.Symbol.StartIndex, 0);
                }

                if (_mode.Count > 0)
                {
                    name = _mode.Peek() + "." + name;
                }

                _navigationTargets.Add(new EditorNavigationTarget(name, navigationType, new SnapshotSpan(_snapshot, span), new SnapshotSpan(_snapshot, seek), glyph));
            }
        }
    }
}
