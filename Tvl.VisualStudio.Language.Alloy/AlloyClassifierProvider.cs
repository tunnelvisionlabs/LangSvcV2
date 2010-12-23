namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IClassifierProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    public sealed class AlloyClassifierProvider : IClassifierProvider
    {
        [Import]
        private IStandardClassificationService StandardClassificationService
        {
            get;
            set;
        }

        [Import]
        private IClassificationTypeRegistryService ClassificationTypeRegistryService
        {
            get;
            set;
        }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                return null;

            return new AlloyClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
