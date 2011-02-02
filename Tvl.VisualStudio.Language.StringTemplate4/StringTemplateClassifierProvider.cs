namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IClassifierProvider))]
    [ContentType(StringTemplateConstants.StringTemplateContentType)]
    public sealed class StringTemplateClassifierProvider : IClassifierProvider
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

            return new StringTemplateClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
