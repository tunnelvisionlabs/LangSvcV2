namespace Tvl.VisualStudio.Language.Go
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;

    internal sealed class GoEditorNavigationSource : IEditorNavigationSource
    {
        private List<IEditorNavigationTarget> _navigationTargets;
        private readonly GoEditorNavigationSourceProvider _provider;
        private readonly Dictionary<int, ImageSource> _glyphCache = new Dictionary<int, ImageSource>();
        private readonly ReaderWriterLockSlim _glyphCacheLock = new ReaderWriterLockSlim();

        public event EventHandler NavigationTargetsChanged;

        public GoEditorNavigationSource(ITextBuffer textBuffer, IBackgroundParser backgroundParser, GoEditorNavigationSourceProvider provider)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (backgroundParser == null)
                throw new ArgumentNullException("backgroundParser");
            if (provider == null)
                throw new ArgumentNullException("provider");
            Contract.EndContractBlock();

            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;
            this._provider = provider;

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
            get
            {
                return _provider.EditorNavigationTypeRegistryService;
            }
        }

        private IGlyphService GlyphService
        {
            get
            {
                return _provider.GlyphService;
            }
        }

        public IEnumerable<IEditorNavigationType> GetNavigationTypes()
        {
            yield return EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
            yield return EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
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
                //// add the Global Scope type
                //{
                //    var name = "Global Scope";
                //    var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                //    var span = new SnapshotSpan(e.Snapshot, new Span(0, e.Snapshot.Length));
                //    var seek = new SnapshotSpan(e.Snapshot, new Span(0, 0));
                //    var glyph = GetGlyph(StandardGlyphGroup.GlyphGroupNamespace, StandardGlyphItem.GlyphItemPublic);
                //    var target = new EditorNavigationTarget(name, navigationType, span, seek, glyph);
                //    navigationTargets.Add(target);
                //}

                var result = antlrParseResultArgs.Result.Tree as CommonTree;
                if (result != null)
                {
                    foreach (CommonTree child in result.Children)
                    {
                        if (child == null || string.IsNullOrEmpty(child.Text))
                            continue;

                        switch (child.Type)
                        {
                        case GoLexer.KW_PACKAGE:
                            {
                                var packageName = ((CommonTree)child.Children[0]).Token.Text;
                                if (string.IsNullOrWhiteSpace(packageName))
                                    continue;

                                var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                                var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                                //Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                //SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                // applies to the whole file
                                var span = new SnapshotSpan(e.Snapshot, new Span(0, e.Snapshot.Length));
                                SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(((CommonTree)child.Children[0]).Token.StartIndex, 0));
                                var glyph = GetGlyph(StandardGlyphGroup.GlyphGroupModule, StandardGlyphItem.GlyphItemPublic);
                                navigationTargets.Add(new EditorNavigationTarget(packageName, navigationType, span, ruleSeek, glyph));
                            }
                            break;

                        case GoLexer.KW_TYPE:
                            // each child tree is a typeSpec, the root of which is an identifier that names the type
                            foreach (CommonTree typeSpec in child.Children)
                            {
                                var typeName = typeSpec.Token.Text;
                                if (string.IsNullOrWhiteSpace(typeName))
                                    continue;

                                var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                                var startToken = antlrParseResultArgs.Tokens[typeSpec.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[typeSpec.TokenStopIndex];
                                Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(typeSpec.Token.StartIndex, 0));
                                var glyph = GetGlyph(StandardGlyphGroup.GlyphGroupMethod, char.IsUpper(typeName[0]) ? StandardGlyphItem.GlyphItemPublic : StandardGlyphItem.GlyphItemPrivate);
                                navigationTargets.Add(new EditorNavigationTarget(typeName, navigationType, ruleSpan, ruleSeek, glyph));
                            }
                            break;

                        case GoLexer.KW_CONST:
                        case GoLexer.KW_VAR:
                            foreach (CommonTree spec in child.Children)
                            {
                                CommonTree decl = (CommonTree)spec.Children[0];
                                foreach (CommonTree nameToken in decl.Children)
                                {
                                    var name = nameToken.Token.Text;
                                    if (string.IsNullOrWhiteSpace(name))
                                        continue;

                                    var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
                                    var startToken = antlrParseResultArgs.Tokens[nameToken.TokenStartIndex];
                                    var stopToken = antlrParseResultArgs.Tokens[nameToken.TokenStopIndex];
                                    Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                    SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                    SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(nameToken.Token.StartIndex, 0));
                                    var group = (child.Type == GoLexer.KW_CONST) ? StandardGlyphGroup.GlyphGroupConstant : StandardGlyphGroup.GlyphGroupVariable;
                                    var item = char.IsUpper(name[0]) ? StandardGlyphItem.GlyphItemPublic : StandardGlyphItem.GlyphItemPrivate;
                                    var glyph = GetGlyph(group, item);
                                    navigationTargets.Add(new EditorNavigationTarget(name, navigationType, ruleSpan, ruleSeek, glyph));
                                }
                            }
                            break;

                        case GoLexer.KW_FUNC:
                            {
                                // the first child is either a receiver (method) or an identifier with the name of the function
                                var token = ((CommonTree)child.Children[0]).Token;
                                if (token.Type == GoLexer.LPAREN)
                                    token = ((CommonTree)child.Children[1]).Token;

                                var functionName = token.Text;
                                if (string.IsNullOrWhiteSpace(functionName))
                                    continue;

                                var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
                                var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                                Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(child.Token.StartIndex, 0));
                                var glyph = GetGlyph(StandardGlyphGroup.GlyphGroupMethod, char.IsUpper(functionName[0]) ? StandardGlyphItem.GlyphItemPublic : StandardGlyphItem.GlyphItemPrivate);
                                navigationTargets.Add(new EditorNavigationTarget(functionName, navigationType, ruleSpan, ruleSeek, glyph));
                            }

                            break;

                        default:
                            continue;
                        }

                        //    if (child.Text == "rule" && child.ChildCount > 0)
                        //    {
                        //        var ruleName = child.GetChild(0).Text;
                        //        if (string.IsNullOrEmpty(ruleName))
                        //            continue;

                        //        var navigationType = char.IsUpper(ruleName[0]) ? _lexerRuleNavigationType : _parserRuleNavigationType;
                        //        IToken startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                        //        IToken stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                        //        Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                        //        SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                        //        SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(((CommonTree)child.GetChild(0)).Token.StartIndex, 0));
                        //        var glyph = char.IsUpper(ruleName[0]) ? _lexerRuleGlyph : _parserRuleGlyph;
                        //        _navigationTargets.Add(new EditorNavigationTarget(ruleName, navigationType, ruleSpan, ruleSeek, glyph));
                        //    }
                        //    else if (child.Text.StartsWith("tokens"))
                        //    {
                        //        foreach (CommonTree tokenChild in child.Children)
                        //        {
                        //            if (tokenChild.Text == "=" && tokenChild.ChildCount == 2)
                        //            {
                        //                var ruleName = tokenChild.GetChild(0).Text;
                        //                if (string.IsNullOrEmpty(ruleName))
                        //                    continue;

                        //                var navigationType = char.IsUpper(ruleName[0]) ? _lexerRuleNavigationType : _parserRuleNavigationType;
                        //                IToken startToken = antlrParseResultArgs.Tokens[tokenChild.TokenStartIndex];
                        //                IToken stopToken = antlrParseResultArgs.Tokens[tokenChild.TokenStopIndex];
                        //                Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                        //                SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                        //                SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(((CommonTree)tokenChild.GetChild(0)).Token.StartIndex, 0));
                        //                var glyph = char.IsUpper(ruleName[0]) ? _lexerRuleGlyph : _parserRuleGlyph;
                        //                _navigationTargets.Add(new EditorNavigationTarget(ruleName, navigationType, ruleSpan, ruleSeek, glyph));
                        //            }
                        //            else if (tokenChild.ChildCount == 0)
                        //            {
                        //                var ruleName = tokenChild.Text;
                        //                if (string.IsNullOrEmpty(ruleName))
                        //                    continue;

                        //                var navigationType = char.IsUpper(ruleName[0]) ? _lexerRuleNavigationType : _parserRuleNavigationType;
                        //                IToken startToken = antlrParseResultArgs.Tokens[tokenChild.TokenStartIndex];
                        //                IToken stopToken = antlrParseResultArgs.Tokens[tokenChild.TokenStopIndex];
                        //                Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                        //                SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                        //                SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(tokenChild.Token.StartIndex, 0));
                        //                var glyph = char.IsUpper(ruleName[0]) ? _lexerRuleGlyph : _parserRuleGlyph;
                        //                _navigationTargets.Add(new EditorNavigationTarget(ruleName, navigationType, ruleSpan, ruleSeek, glyph));
                        //            }
                        //        }
                        //    }
                    }
                }
            }

            this._navigationTargets = navigationTargets;
            OnNavigationTargetsChanged(EventArgs.Empty);
        }

        private ImageSource GetGlyph(StandardGlyphGroup group, StandardGlyphItem item)
        {
            bool entered = false;
            try
            {
                entered = _glyphCacheLock.TryEnterUpgradeableReadLock(50);
                if (!entered)
                    return null;

                int key = (int)group << 16 + (int)item;
                ImageSource source;
                if (!_glyphCache.TryGetValue(key, out source))
                {
                    _glyphCacheLock.EnterWriteLock();
                    try
                    {
                        // create the glyph on the UI thread
                        Dispatcher dispatcher = _provider.Dispatcher;
                        if (dispatcher == null)
                        {
                            _glyphCache[key] = source = null;
                        }
                        else
                        {
                            dispatcher.Invoke((Action)(
                                () =>
                                {
                                    _glyphCache[key] = source = GlyphService.GetGlyph(group, item);
                                }));
                        }
                    }
                    finally
                    {
                        _glyphCacheLock.ExitWriteLock();
                    }
                }

                return source;
            }
            finally
            {
                if (entered)
                {
                    _glyphCacheLock.ExitUpgradeableReadLock();
                }
            }
        }
    }
}
