namespace Tvl.VisualStudio.Language.Alloy.Experimental
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using Tvl.VisualStudio.Text.Navigation;
    using Action = System.Action;
    using Dispatcher = System.Windows.Threading.Dispatcher;
    using ImageSource = System.Windows.Media.ImageSource;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    internal sealed class AlloyAtnEditorNavigationSourceProvider : IEditorNavigationSourceProvider, IGlyphService
    {
        private readonly Dictionary<int, ImageSource> _glyphCache = new Dictionary<int, ImageSource>();
        private readonly ReaderWriterLockSlim _glyphCacheLock = new ReaderWriterLockSlim();

        public AlloyAtnEditorNavigationSourceProvider()
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

        [Import(PredefinedTaskSchedulers.BackgroundIntelliSense)]
        public TaskScheduler BackgroundIntelliSenseTaskScheduler
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

        public Dispatcher Dispatcher
        {
            get;
            private set;
        }

        public IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer)
        {
            return new AlloyAtnEditorNavigationSource(textBuffer, this);
        }

        public ImageSource GetGlyph(StandardGlyphGroup group, StandardGlyphItem item)
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
