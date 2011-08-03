namespace Tvl.VisualStudio.Language.Php
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITextStructureNavigatorProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    internal class PhpTextStructureNavigatorProvider : ITextStructureNavigatorProvider
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
            IContentType contentType = ContentTypeRegistryService.GetContentType(PhpConstants.PhpContentType);
            IContentType baseContentType = contentType.BaseTypes.First();
            ITextStructureNavigator delegateNavigator = TextStructureNavigatorSelectorService.CreateTextStructureNavigator(textBuffer, baseContentType);
            return new PhpTextStructureNavigator(textBuffer, delegateNavigator);
        }
    }
}
