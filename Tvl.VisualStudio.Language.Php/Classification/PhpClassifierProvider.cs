namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Classification;

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

        protected override IClassifier GetClassifierImpl([NotNull] ITextBuffer textBuffer)
        {
            if (!textBuffer.ContentType.IsOfType(PhpConstants.PhpContentType))
                return null;

            return new V4PhpClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
