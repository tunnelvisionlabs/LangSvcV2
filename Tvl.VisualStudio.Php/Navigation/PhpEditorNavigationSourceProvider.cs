namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Action = System.Action;
    using ContentTypeAttribute = Microsoft.VisualStudio.Utilities.ContentTypeAttribute;
    using Dispatcher = System.Windows.Threading.Dispatcher;
    using IEditorNavigationSource = Tvl.VisualStudio.Text.Navigation.IEditorNavigationSource;
    using IEditorNavigationSourceProvider = Tvl.VisualStudio.Text.Navigation.IEditorNavigationSourceProvider;
    using IEditorNavigationTypeRegistryService = Tvl.VisualStudio.Text.Navigation.IEditorNavigationTypeRegistryService;
    using IGlyphService = Microsoft.VisualStudio.Language.Intellisense.IGlyphService;
    using ImageSource = System.Windows.Media.ImageSource;
    using IOutputWindowService = Tvl.VisualStudio.Shell.OutputWindow.IOutputWindowService;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;
    using ITextDocumentFactoryService = Microsoft.VisualStudio.Text.ITextDocumentFactoryService;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;
    using StandardGlyphGroup = Microsoft.VisualStudio.Language.Intellisense.StandardGlyphGroup;
    using StandardGlyphItem = Microsoft.VisualStudio.Language.Intellisense.StandardGlyphItem;

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    internal sealed class PhpEditorNavigationSourceProvider : IEditorNavigationSourceProvider
    {
        private readonly Dictionary<int, ImageSource> _glyphCache = new Dictionary<int, ImageSource>();
        private readonly ReaderWriterLockSlim _glyphCacheLock = new ReaderWriterLockSlim();

        public PhpEditorNavigationSourceProvider()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        [Import]
        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService
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
        private IGlyphService GlyphService
        {
            get;
            set;
        }

        private Dispatcher Dispatcher
        {
            get;
            set;
        }

        public IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer)
        {
            return new PhpEditorNavigationSource(textBuffer, this);
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
