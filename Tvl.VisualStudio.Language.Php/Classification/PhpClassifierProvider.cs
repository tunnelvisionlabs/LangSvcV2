namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Classification;
    using StringComparison = System.StringComparison;
    using Path = System.IO.Path;

    [Export(typeof(IClassifierProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    public sealed class PhpClassifierProvider : LanguageClassifierProvider<PhpLanguagePackage>
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService
        {
            get;
            private set;
        }

        protected override IClassifier GetClassifierImpl(ITextBuffer textBuffer)
        {
            ITextDocument textDocument;
            if (TextDocumentFactoryService.TryGetTextDocument(textBuffer, out textDocument))
            {
                if (!string.IsNullOrEmpty(textDocument.FilePath)
                    && Path.GetExtension(textDocument.FilePath).Equals(PhpConstants.Php5FileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return new V4PhpClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
                }
            }

            return new PhpClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
