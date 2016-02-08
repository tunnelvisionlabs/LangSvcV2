namespace Tvl.VisualStudio.Language.Antlr3
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITextStructureNavigatorProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    internal class AntlrTextStructureNavigatorProvider : ITextStructureNavigatorProvider
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
            IContentType contentType = ContentTypeRegistryService.GetContentType(AntlrConstants.AntlrContentType);
            IContentType baseContentType = contentType.BaseTypes.First();
            ITextStructureNavigator delegateNavigator = TextStructureNavigatorSelectorService.CreateTextStructureNavigator(textBuffer, baseContentType);
            return new AntlrTextStructureNavigator(textBuffer, delegateNavigator);
        }
    }
}
