namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IClassifierProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    public sealed class PhpClassifierProvider : IClassifierProvider
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

            return new PhpClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
