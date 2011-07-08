namespace JavaLanguageService
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IClassifierProvider))]
    [ContentType(Constants.JavaContentType)]
    public sealed class JavaClassifierProvider : IClassifierProvider
    {
        [Import]
        private IStandardClassificationService StandardClassificationService
        {
            get;
            set;
        }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                return null;

            return new JavaClassifier(textBuffer, StandardClassificationService);
        }
    }
}
