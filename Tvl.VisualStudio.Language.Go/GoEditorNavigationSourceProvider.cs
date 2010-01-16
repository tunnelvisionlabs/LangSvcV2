namespace Tvl.VisualStudio.Language.Go
{
    using System.ComponentModel.Composition;
    using System.Windows.Threading;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType(GoConstants.GoContentType)]
    public sealed class GoEditorNavigationSourceProvider : IEditorNavigationSourceProvider
    {
        public GoEditorNavigationSourceProvider()
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

        public Dispatcher Dispatcher
        {
            get;
            private set;
        }

        public IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer)
        {
            var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(textBuffer);
            if (backgroundParser == null)
                return null;

            return new GoEditorNavigationSource(textBuffer, backgroundParser, this);
        }
    }
}
