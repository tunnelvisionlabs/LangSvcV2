namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Classification;

    [Export(typeof(IClassifierProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    public sealed class PhpClassifierProvider : LanguageClassifierProvider<PhpLanguagePackage>
    {
        protected override IClassifier GetClassifierImpl(ITextBuffer textBuffer)
        {
            return new PhpClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
