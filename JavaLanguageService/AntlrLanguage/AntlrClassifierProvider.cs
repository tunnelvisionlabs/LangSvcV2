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
        public IStandardClassificationService StandardClassificationService;

        [Import]
        public IClassificationTypeRegistryService ClassificationTypeRegistryService;

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                return null;

            return new AntlrClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
