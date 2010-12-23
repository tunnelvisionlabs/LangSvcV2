namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;

    internal sealed class AlloyEditorNavigationSource : IEditorNavigationSource
    {
        private List<IEditorNavigationTarget> _navigationTargets;
        private readonly AlloyEditorNavigationSourceProvider _provider;
        private readonly Dictionary<int, ImageSource> _glyphCache = new Dictionary<int, ImageSource>();
        private readonly ReaderWriterLockSlim _glyphCacheLock = new ReaderWriterLockSlim();

        public event EventHandler NavigationTargetsChanged;

        public AlloyEditorNavigationSource(ITextBuffer textBuffer, IBackgroundParser backgroundParser, AlloyEditorNavigationSourceProvider provider)
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
                AlloyParser.compilationUnit_return resultArgs = antlrParseResultArgs.Result as AlloyParser.compilationUnit_return;
                var result = resultArgs.Tree;
                if (result != null && result.Children != null)
                {
                    foreach (var child in result.Children.OfType<CommonTree>())
                    {
                        switch (child.Type)
                        {
                        case AlloyParser.KW_MODULE:
                            continue;

                        case AlloyParser.KW_SIG:
                        case AlloyParser.KW_ENUM:
                            {
                                CommonTree nameTree = child.Children.OfType<CommonTree>().SkipWhile(IsSigQualifier).FirstOrDefault();
                                IEnumerable<CommonTree> names;
                                if (nameTree.Type == AlloyParser.COMMA)
                                    names = nameTree.Children.OfType<CommonTree>();
                                else
                                    names = Enumerable.Repeat(nameTree, 1);

                                foreach (var name in names)
                                {
                                    var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                                    var startToken = antlrParseResultArgs.Tokens[name.TokenStartIndex];
                                    var stopToken = antlrParseResultArgs.Tokens[name.TokenStopIndex];
                                    Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                    SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                    SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(name.Token.StartIndex, 0));
                                    var group = (child.Type == AlloyParser.KW_SIG) ? StandardGlyphGroup.GlyphGroupStruct : StandardGlyphGroup.GlyphGroupEnum;
                                    var item = StandardGlyphItem.GlyphItemPublic;
                                    var glyph = GetGlyph(group, item);
                                    navigationTargets.Add(new EditorNavigationTarget(name.Text, navigationType, ruleSpan, ruleSeek, glyph));
                                }
                            }
                            continue;

                        case AlloyParser.KW_FUN:
                        case AlloyParser.KW_PRED:
                        case AlloyParser.KW_FACT:
                        case AlloyParser.KW_ASSERT:
                            //{
                            //    CommonTree nameTree = child.Children.OfType<CommonTree>().SkipWhile(IsSigQualifier).FirstOrDefault();
                            //    IEnumerable<CommonTree> names;
                            //    if (nameTree.Type == AlloyParser.COMMA)
                            //        names = nameTree.Children.OfType<CommonTree>();
                            //    else
                            //        names = Enumerable.Repeat(nameTree, 1);

                            //    foreach (var name in names)
                            //    {
                            //        var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                            //        var startToken = antlrParseResultArgs.Tokens[name.TokenStartIndex];
                            //        var stopToken = antlrParseResultArgs.Tokens[name.TokenStopIndex];
                            //        Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                            //        SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                            //        SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(name.Token.StartIndex, 0));
                            //        var group = (child.Type == AlloyParser.KW_SIG) ? StandardGlyphGroup.GlyphGroupStruct : StandardGlyphGroup.GlyphGroupEnum;
                            //        var item = StandardGlyphItem.GlyphItemPublic;
                            //        var glyph = GetGlyph(group, item);
                            //        navigationTargets.Add(new EditorNavigationTarget(name.Text, navigationType, ruleSpan, ruleSeek, glyph));
                            //    }
                            //}
                            break;

                        default:
                            continue;
                        }
                    }
                }
            }

            this._navigationTargets = navigationTargets;
            OnNavigationTargetsChanged(EventArgs.Empty);
        }

        private static bool IsSigQualifier(ITree tree)
        {
            if (tree == null)
                return false;

            switch (tree.Type)
            {
            case AlloyParser.KW_ABSTRACT:
            case AlloyParser.KW_LONE:
            case AlloyParser.KW_ONE:
            case AlloyParser.KW_SOME:
            case AlloyParser.KW_PRIVATE:
                return true;

            default:
                return false;
            }
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
