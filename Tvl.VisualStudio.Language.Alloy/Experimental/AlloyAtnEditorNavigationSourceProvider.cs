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
    using IDispatcherGlyphService = Tvl.VisualStudio.Language.Intellisense.IDispatcherGlyphService;

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    internal sealed class AlloyAtnEditorNavigationSourceProvider : IEditorNavigationSourceProvider
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
        public IDispatcherGlyphService GlyphService
        {
            get;
            private set;
        }

        public IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer)
        {
            return new AlloyAtnEditorNavigationSource(textBuffer, this);
        }
    }
}
