namespace Tvl.VisualStudio.Language.Go
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;
    using IDispatcherGlyphService = Tvl.VisualStudio.Language.Intellisense.IDispatcherGlyphService;

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType(GoConstants.GoContentType)]
    public sealed class GoEditorNavigationSourceProvider : IEditorNavigationSourceProvider
    {
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
        public IDispatcherGlyphService GlyphService
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
