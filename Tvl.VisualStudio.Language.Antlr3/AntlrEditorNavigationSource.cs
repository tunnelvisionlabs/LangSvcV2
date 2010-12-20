namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;

    internal sealed class AntlrEditorNavigationSource : IEditorNavigationSource
    {
        private IEditorNavigationType _parserRuleNavigationType;
        private IEditorNavigationType _lexerRuleNavigationType;
        private ImageSource _lexerRuleGlyph;
        private ImageSource _parserRuleGlyph;
        private List<IEditorNavigationTarget> _navigationTargets;

        public event EventHandler NavigationTargetsChanged;

        public AntlrEditorNavigationSource(ITextBuffer textBuffer, IBackgroundParser backgroundParser, IEditorNavigationTypeRegistryService editorNavigationTypeRegistryService)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (backgroundParser == null)
                throw new ArgumentNullException("backgroundParser");
            if (editorNavigationTypeRegistryService == null)
                throw new ArgumentNullException("editorNavigationTypeRegistryService");
            Contract.EndContractBlock();

            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;
            this.EditorNavigationTypeRegistryService = editorNavigationTypeRegistryService;

            this._parserRuleNavigationType = this.EditorNavigationTypeRegistryService.GetEditorNavigationType(AntlrEditorNavigationTypeNames.ParserRule);
            this._lexerRuleNavigationType = this.EditorNavigationTypeRegistryService.GetEditorNavigationType(AntlrEditorNavigationTypeNames.LexerRule);

            string assemblyName = typeof(AntlrEditorNavigationSource).Assembly.GetName().Name;
            this._lexerRuleGlyph = new BitmapImage(new Uri(string.Format("pack://application:,,,/{0};component/Resources/lexericon.png", assemblyName)));
            this._parserRuleGlyph = new BitmapImage(new Uri(string.Format("pack://application:,,,/{0};component/Resources/parsericon.png", assemblyName)));

            this._navigationTargets = new List<IEditorNavigationTarget>();

            this.BackgroundParser.ParseComplete += OnBackgroundParseComplete;
        }

        private ITextBuffer TextBuffer
        {
            get;
            set;
        }

        private IBackgroundParser BackgroundParser
        {
            get;
            set;
        }

        private IEditorNavigationTypeRegistryService EditorNavigationTypeRegistryService
        {
            get;
            set;
        }

        public IEnumerable<IEditorNavigationType> GetNavigationTypes()
        {
            yield return _parserRuleNavigationType;
            yield return _lexerRuleNavigationType;
        }

        public IEnumerable<IEditorNavigationTarget> GetNavigationTargets()
        {
            return _navigationTargets;
        }

        private void OnNavigationTargetsChanged(EventArgs e)
        {
            var t = NavigationTargetsChanged;
            if (t != null)
                t(this, e);
        }

        private void OnBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
            List<IEditorNavigationTarget> navigationTargets = new List<IEditorNavigationTarget>();
            if (antlrParseResultArgs != null)
            {
                dynamic resultArgs = antlrParseResultArgs.Result;
                var result = resultArgs.Tree as CommonTree;
                if (result != null)
                {
                    foreach (CommonTree child in result.Children)
                    {
                        if (child == null || string.IsNullOrEmpty(child.Text))
                            continue;

                        if (child.Text == "rule" && child.ChildCount > 0)
                        {
                            var ruleName = child.GetChild(0).Text;
                            if (string.IsNullOrEmpty(ruleName))
                                continue;

                            if (ruleName == "Tokens")
                                continue;

                            var navigationType = char.IsUpper(ruleName[0]) ? _lexerRuleNavigationType : _parserRuleNavigationType;
                            IToken startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                            IToken stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                            SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                            SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(((CommonTree)child.GetChild(0)).Token.StartIndex, 0));
                            var glyph = char.IsUpper(ruleName[0]) ? _lexerRuleGlyph : _parserRuleGlyph;
                            navigationTargets.Add(new EditorNavigationTarget(ruleName, navigationType, ruleSpan, ruleSeek, glyph));
                        }
                        else if (child.Text.StartsWith("tokens"))
                        {
                            foreach (CommonTree tokenChild in child.Children)
                            {
                                if (tokenChild.Text == "=" && tokenChild.ChildCount == 2)
                                {
                                    var ruleName = tokenChild.GetChild(0).Text;
                                    if (string.IsNullOrEmpty(ruleName))
                                        continue;

                                    var navigationType = char.IsUpper(ruleName[0]) ? _lexerRuleNavigationType : _parserRuleNavigationType;
                                    IToken startToken = antlrParseResultArgs.Tokens[tokenChild.TokenStartIndex];
                                    IToken stopToken = antlrParseResultArgs.Tokens[tokenChild.TokenStopIndex];
                                    Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                    SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                    SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(((CommonTree)tokenChild.GetChild(0)).Token.StartIndex, 0));
                                    var glyph = char.IsUpper(ruleName[0]) ? _lexerRuleGlyph : _parserRuleGlyph;
                                    navigationTargets.Add(new EditorNavigationTarget(ruleName, navigationType, ruleSpan, ruleSeek, glyph));
                                }
                                else if (tokenChild.ChildCount == 0)
                                {
                                    var ruleName = tokenChild.Text;
                                    if (string.IsNullOrEmpty(ruleName))
                                        continue;

                                    var navigationType = char.IsUpper(ruleName[0]) ? _lexerRuleNavigationType : _parserRuleNavigationType;
                                    IToken startToken = antlrParseResultArgs.Tokens[tokenChild.TokenStartIndex];
                                    IToken stopToken = antlrParseResultArgs.Tokens[tokenChild.TokenStopIndex];
                                    Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                    SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                    SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(tokenChild.Token.StartIndex, 0));
                                    var glyph = char.IsUpper(ruleName[0]) ? _lexerRuleGlyph : _parserRuleGlyph;
                                    navigationTargets.Add(new EditorNavigationTarget(ruleName, navigationType, ruleSpan, ruleSeek, glyph));
                                }
                            }
                        }

                    }
                }
            }

            this._navigationTargets = navigationTargets;
            OnNavigationTargetsChanged(EventArgs.Empty);
        }
    }
}
