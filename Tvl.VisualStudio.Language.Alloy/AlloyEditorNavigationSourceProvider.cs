namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows.Threading;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;
    using Action = System.Action;
    using ImageSource = System.Windows.Media.ImageSource;
    using IOutputWindowService = Tvl.VisualStudio.Shell.OutputWindow.IOutputWindowService;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    public sealed class AlloyEditorNavigationSourceProvider : IEditorNavigationSourceProvider
    {
        private readonly Dictionary<int, ImageSource> _glyphCache = new Dictionary<int, ImageSource>();
        private readonly ReaderWriterLockSlim _glyphCacheLock = new ReaderWriterLockSlim();

        public AlloyEditorNavigationSourceProvider()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        [Import]
        public IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            private set;
        }

        [Import]
        public IEditorNavigationTypeRegistryService EditorNavigationTypeRegistryService
        {
            get;
            private set;
        }

        [Import]
        public IGlyphService GlyphService
        {
            get;
            private set;
        }

        [Import]
        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        public Dispatcher Dispatcher
        {
            get;
            private set;
        }

        public IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer)
        {
            var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(textBuffer) as AlloyBackgroundParser;
            if (backgroundParser == null)
                return null;

            return new AlloyEditorNavigationSource(textBuffer, backgroundParser, this);
        }

        internal ImageSource GetGlyph(StandardGlyphGroup group, StandardGlyphItem item)
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
                        Dispatcher dispatcher = Dispatcher;
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
