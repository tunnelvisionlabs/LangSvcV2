namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Classification;

    [Export(typeof(IClassifierProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    public sealed class AlloyClassifierProvider : LanguageClassifierProvider<AlloyLanguagePackage>
    {
        protected override IClassifier GetClassifierImpl(ITextBuffer textBuffer)
        {
            return new AlloyClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
