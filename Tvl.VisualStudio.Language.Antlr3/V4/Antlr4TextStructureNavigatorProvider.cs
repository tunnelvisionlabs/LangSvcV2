namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITextStructureNavigatorProvider))]
    [ContentType(Antlr4Constants.AntlrContentType)]
    internal class Antlr4TextStructureNavigatorProvider : ITextStructureNavigatorProvider
    {
        [Import]
        public IContentTypeRegistryService ContentTypeRegistryService
        {
            get;
            private set;
        }

        [Import]
        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get;
            private set;
        }

        public ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer)
        {
            IContentType contentType = ContentTypeRegistryService.GetContentType(Antlr4Constants.AntlrContentType);
            IContentType baseContentType = contentType.BaseTypes.First();
            ITextStructureNavigator delegateNavigator = TextStructureNavigatorSelectorService.CreateTextStructureNavigator(textBuffer, baseContentType);
            return new Antlr4TextStructureNavigator(textBuffer, delegateNavigator);
        }
    }
}
