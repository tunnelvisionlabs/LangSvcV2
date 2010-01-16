namespace Tvl.VisualStudio.Language.Go
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IClassifierProvider))]
    [ContentType(GoConstants.GoContentType)]
    public sealed class GoClassifierProvider : IClassifierProvider
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

            return new GoClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
