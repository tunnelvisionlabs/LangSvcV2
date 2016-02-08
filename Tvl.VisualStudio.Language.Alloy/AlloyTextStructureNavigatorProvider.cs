namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITextStructureNavigatorProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    internal class AlloyTextStructureNavigatorProvider : ITextStructureNavigatorProvider, IPartImportsSatisfiedNotification
    {
        private IContentType _anyContentType;

        [Import]
        private ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get;
            set;
        }

        [Import]
        private IContentTypeRegistryService ContentTypeRegistryService
        {
            get;
            set;
        }

        private IContentType AnyContentType
        {
            get
            {
                return _anyContentType;
            }
        }

        public ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");

            ITextStructureNavigator delegateTextStructureNavigator = TextStructureNavigatorSelectorService.CreateTextStructureNavigator(textBuffer, AnyContentType);
            return new AlloyTextStructureNavigator(textBuffer, delegateTextStructureNavigator);
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            _anyContentType = ContentTypeRegistryService.GetContentType("any");
        }
    }
}
