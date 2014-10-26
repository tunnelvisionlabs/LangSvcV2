namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Navigation;
    using IBackgroundParserFactoryService = Tvl.VisualStudio.Language.Parsing.IBackgroundParserFactoryService;

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType(Antlr4Constants.AntlrContentType)]
    public sealed class Antlr4EditorNavigationSourceProvider : IEditorNavigationSourceProvider
    {
        private readonly IBackgroundParserFactoryService _backgroundParserFactoryService;
        private readonly IEditorNavigationTypeRegistryService _editorNavigationTypeRegistryService;

        [ImportingConstructor]
        public Antlr4EditorNavigationSourceProvider(IBackgroundParserFactoryService backgroundParserFactoryService, IEditorNavigationTypeRegistryService editorNavigationTypeRegistryService)
        {
            _backgroundParserFactoryService = backgroundParserFactoryService;
            _editorNavigationTypeRegistryService = editorNavigationTypeRegistryService;
        }

        public IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get
            {
                return _backgroundParserFactoryService;
            }
        }

        public IEditorNavigationTypeRegistryService EditorNavigationTypeRegistryService
        {
            get
            {
                return _editorNavigationTypeRegistryService;
            }
        }

        public IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer)
        {
            var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(textBuffer) as Antlr4BackgroundParser;
            if (backgroundParser == null)
                return null;

            return new Antlr4EditorNavigationSource(this, textBuffer);
        }
    }
}
