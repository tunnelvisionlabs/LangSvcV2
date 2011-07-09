namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;
    using ImageSource = System.Windows.Media.ImageSource;
    using StandardGlyphGroup = Microsoft.VisualStudio.Language.Intellisense.StandardGlyphGroup;
    using StandardGlyphItem = Microsoft.VisualStudio.Language.Intellisense.StandardGlyphItem;
    using Antlr.Runtime.Tree;

    internal sealed class PhpEditorNavigationSource : IEditorNavigationSource
    {
        private readonly ITextBuffer _textBuffer;
        private readonly PhpEditorNavigationBackgroundParser _backgroundParser;
        private readonly PhpEditorNavigationSourceProvider _provider;

        private List<IEditorNavigationTarget> _navigationTargets;

        public event EventHandler NavigationTargetsChanged;

        public PhpEditorNavigationSource(ITextBuffer textBuffer, PhpEditorNavigationSourceProvider provider)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");

            this._textBuffer = textBuffer;
            this._backgroundParser = PhpEditorNavigationBackgroundParser.CreateParser(textBuffer, provider.BackgroundIntelliSenseTaskScheduler, provider.OutputWindowService, provider.TextDocumentFactoryService);
            this._provider = provider;

            this.BackgroundParser.ParseComplete += HandleBackgroundParseComplete;
            this.BackgroundParser.RequestParse(false);
        }

        internal PhpEditorNavigationSourceProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        internal PhpEditorNavigationBackgroundParser BackgroundParser
        {
            get
            {
                return _backgroundParser;
            }
        }

        public IEnumerable<IEditorNavigationType> GetNavigationTypes()
        {
            yield return _provider.EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
            yield return _provider.EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
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
            PhpEditorNavigationParseResultEventArgs antlrParseResultArgs = e as PhpEditorNavigationParseResultEventArgs;
            if (antlrParseResultArgs == null)
                return;

            UpdateNavigationTargets(antlrParseResultArgs);
        }

        private void UpdateNavigationTargets(PhpEditorNavigationParseResultEventArgs antlrParseResultArgs)
        {
            Contract.Requires(antlrParseResultArgs != null);

            List<IEditorNavigationTarget> navigationTargets = new List<IEditorNavigationTarget>();

            // always add the "global scope" element
            {
                string name = "Global Scope";
                IEditorNavigationType editorNavigationType = Provider.EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                SnapshotSpan span = new SnapshotSpan(antlrParseResultArgs.Snapshot, 0, antlrParseResultArgs.Snapshot.Length);
                SnapshotSpan seek = new SnapshotSpan(span.Start, 0);
                ImageSource glyph = Provider.GetGlyph(StandardGlyphGroup.GlyphGroupNamespace, StandardGlyphItem.GlyphItemPublic);
                NavigationTargetStyle style = NavigationTargetStyle.None;
                navigationTargets.Add(new EditorNavigationTarget(name, editorNavigationType, span, seek, glyph, style));
            }

            if (antlrParseResultArgs != null)
            {
                ITextSnapshot snapshot = antlrParseResultArgs.Snapshot;

                foreach (var tree in antlrParseResultArgs.NavigationTrees)
                {
                    switch (tree.Type)
                    {
                    case PhpEditorNavigationParser.CLASS_IDENTIFIER:
                    case PhpEditorNavigationParser.INTERFACE_IDENTIFIER:
                        {
                            CommonTree child = tree;
                            if (child != null)
                            {
                                string name = child.Token.Text;
                                for (ITree parent = child.Parent; parent != null; parent = parent.Parent)
                                {
                                    switch (parent.Type)
                                    {
                                    case PhpEditorNavigationParser.INTERFACE_IDENTIFIER:
                                    case PhpEditorNavigationParser.CLASS_IDENTIFIER:
                                        name = parent.Text + "." + name;
                                        continue;

                                    default:
                                        continue;
                                    }
                                }

                                IEditorNavigationType navigationType = Provider.EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                                var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                                SnapshotSpan span = new SnapshotSpan(snapshot, new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1));
                                SnapshotSpan seek = new SnapshotSpan(snapshot, new Span(child.Token.StartIndex, 0));

                                StandardGlyphGroup glyphGroup;
                                switch (child.Type)
                                {
                                //case PhpEditorNavigationParser.ENUM_IDENTIFIER:
                                //    glyphGroup = StandardGlyphGroup.GlyphGroupEnum;
                                //    break;

                                case PhpEditorNavigationParser.INTERFACE_IDENTIFIER:
                                    glyphGroup = StandardGlyphGroup.GlyphGroupInterface;
                                    break;

                                case PhpEditorNavigationParser.CLASS_IDENTIFIER:
                                default:
                                    glyphGroup = StandardGlyphGroup.GlyphGroupClass;
                                    break;
                                }

                                //StandardGlyphItem glyphItem = GetGlyphItemFromChildModifier(child);
                                StandardGlyphItem glyphItem = StandardGlyphItem.GlyphItemPublic;
                                ImageSource glyph = _provider.GetGlyph(glyphGroup, glyphItem);
                                NavigationTargetStyle style = NavigationTargetStyle.None;
                                navigationTargets.Add(new EditorNavigationTarget(name, navigationType, span, seek, glyph, style));
                            }
                        }

                        break;

                    case PhpEditorNavigationParser.FUNCTION_IDENTIFIER:
                        {
                            string name = tree.Token.Text;
                            IEnumerable<string> args = ProcessArguments((CommonTree)tree.GetFirstChildWithType(PhpEditorNavigationParser.FORMAL_PARAMETERS));
                            string sig = string.Format("{0}({1})", name, string.Join(", ", args));
                            IEditorNavigationType navigationType = Provider.EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
                            var startToken = antlrParseResultArgs.Tokens[tree.TokenStartIndex];
                            var stopToken = antlrParseResultArgs.Tokens[tree.TokenStopIndex];
                            SnapshotSpan span = new SnapshotSpan(snapshot, new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1));
                            SnapshotSpan seek = new SnapshotSpan(snapshot, new Span(tree.Token.StartIndex, 0));
                            StandardGlyphGroup glyphGroup = StandardGlyphGroup.GlyphGroupMethod;
                            //StandardGlyphItem glyphItem = GetGlyphItemFromChildModifier(tree);
                            StandardGlyphItem glyphItem = StandardGlyphItem.GlyphItemPublic;
                            ImageSource glyph = _provider.GetGlyph(glyphGroup, glyphItem);
                            NavigationTargetStyle style = NavigationTargetStyle.None;
                            navigationTargets.Add(new EditorNavigationTarget(sig, navigationType, span, seek, glyph, style));
                        }

                        break;
                    }
                }
#if false
                IAstRuleReturnScope resultArgs = antlrParseResultArgs.Result as IAstRuleReturnScope;
                var result = resultArgs != null ? resultArgs.Tree as CommonTree : null;
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
                            SnapshotSpan ruleSpan = new SnapshotSpan(antlrParseResultArgs.Snapshot, span);
                            SnapshotSpan ruleSeek = new SnapshotSpan(antlrParseResultArgs.Snapshot, new Span(((CommonTree)child.GetChild(0)).Token.StartIndex, 0));
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
                                    SnapshotSpan ruleSpan = new SnapshotSpan(antlrParseResultArgs.Snapshot, span);
                                    SnapshotSpan ruleSeek = new SnapshotSpan(antlrParseResultArgs.Snapshot, new Span(((CommonTree)tokenChild.GetChild(0)).Token.StartIndex, 0));
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
                                    SnapshotSpan ruleSpan = new SnapshotSpan(antlrParseResultArgs.Snapshot, span);
                                    SnapshotSpan ruleSeek = new SnapshotSpan(antlrParseResultArgs.Snapshot, new Span(tokenChild.Token.StartIndex, 0));
                                    var glyph = char.IsUpper(ruleName[0]) ? _lexerRuleGlyph : _parserRuleGlyph;
                                    navigationTargets.Add(new EditorNavigationTarget(ruleName, navigationType, ruleSpan, ruleSeek, glyph));
                                }
                            }
                        }

                    }
                }
#endif
            }

            this._navigationTargets = navigationTargets;
            OnNavigationTargetsChanged(EventArgs.Empty);
        }

        private IEnumerable<string> ProcessArguments(CommonTree tree)
        {
            foreach (CommonTree argTree in tree.Children)
            {
                if (argTree.Type != PhpEditorNavigationParser.PARAMETER_IDENTIFIER)
                    continue;

                bool byRef = false;
                if (argTree.ChildCount > 0 && argTree.GetChild(0).Type == PhpEditorNavigationParser.AND)
                    byRef = true;

                if (byRef)
                    yield return "&" + argTree.Text;
                else
                    yield return argTree.Text;
            }
        }
    }
}
