namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System.ComponentModel.Composition;

    using ContentTypeAttribute = Microsoft.VisualStudio.Utilities.ContentTypeAttribute;
    using IDispatcherGlyphService = Tvl.VisualStudio.Language.Intellisense.IDispatcherGlyphService;
    using IEditorNavigationSource = Tvl.VisualStudio.Text.Navigation.IEditorNavigationSource;
    using IEditorNavigationSourceProvider = Tvl.VisualStudio.Text.Navigation.IEditorNavigationSourceProvider;
    using IEditorNavigationTypeRegistryService = Tvl.VisualStudio.Text.Navigation.IEditorNavigationTypeRegistryService;
    using IOutputWindowService = Tvl.VisualStudio.OutputWindow.Interfaces.IOutputWindowService;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;
    using ITextDocumentFactoryService = Microsoft.VisualStudio.Text.ITextDocumentFactoryService;
    using PredefinedTaskSchedulers = Tvl.VisualStudio.Shell.PredefinedTaskSchedulers;
    using TaskScheduler = System.Threading.Tasks.TaskScheduler;

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
