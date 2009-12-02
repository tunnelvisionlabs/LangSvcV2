namespace JavaLanguageService.AntlrLanguage
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IClassifierProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    public sealed class AntlrClassifierProvider : IClassifierProvider
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

            return new AntlrClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
