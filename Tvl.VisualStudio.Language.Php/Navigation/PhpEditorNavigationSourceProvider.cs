namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Action = System.Action;
    using PredefinedTaskSchedulers = Tvl.VisualStudio.Shell.PredefinedTaskSchedulers;
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
    using TaskScheduler = System.Threading.Tasks.TaskScheduler;
    using IDispatcherGlyphService = Tvl.VisualStudio.Language.Intellisense.IDispatcherGlyphService;

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    internal sealed class PhpEditorNavigationSourceProvider : IEditorNavigationSourceProvider
    {
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

        [Import(PredefinedTaskSchedulers.BackgroundIntelliSense)]
        public TaskScheduler BackgroundIntelliSenseTaskScheduler
        {
            get;
            private set;
        }

        [Import]
        public IDispatcherGlyphService GlyphService
        {
            get;
            private set;
        }

        public IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer)
        {
            return new PhpEditorNavigationSource(textBuffer, this);
        }
    }
}
