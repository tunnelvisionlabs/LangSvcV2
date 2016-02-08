namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;

    using IDispatcherGlyphService = Tvl.VisualStudio.Language.Intellisense.IDispatcherGlyphService;
    using IOutputWindowService = Tvl.VisualStudio.OutputWindow.Interfaces.IOutputWindowService;

#if false
    [Export(typeof(IEditorNavigationSourceProvider))]
#endif
    [ContentType(AlloyConstants.AlloyContentType)]
    public sealed class AlloyEditorNavigationSourceProvider : IEditorNavigationSourceProvider
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

        [Import]
        public IOutputWindowService OutputWindowService
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
    }
}
