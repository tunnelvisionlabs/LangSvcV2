namespace Tvl.VisualStudio.Language.Antlr3
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;

    //public sealed class EditorNavigationTypeAttribute : MultipleBaseMetadataAttribute
    //{
    //    public EditorNavigationTypeAttribute(string type)
    //    {
    //        this.EditorNavigationType = type;
    //    }

    //    public string EditorNavigationType
    //    {
    //        get;
    //        private set;
    //    }
    //}

    [Export(typeof(IEditorNavigationSourceProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    //[EditorNavigationType(AntlrEditorNavigationTypeNames.ParserRule)]
    //[EditorNavigationType(AntlrEditorNavigationTypeNames.LexerRule)]
    public sealed class AntlrEditorNavigationSourceProvider : IEditorNavigationSourceProvider
    {
        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        [Import]
        private IEditorNavigationTypeRegistryService EditorNavigationTypeRegistryService
        {
            get;
            set;
        }

        public IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer)
        {
            var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(textBuffer) as AntlrBackgroundParser;
            if (backgroundParser == null)
                return null;

            return new AntlrEditorNavigationSource(textBuffer, backgroundParser, EditorNavigationTypeRegistryService);
        }
    }
}
