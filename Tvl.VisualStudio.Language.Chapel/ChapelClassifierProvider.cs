namespace Tvl.VisualStudio.Language.Chapel
{
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Classification;

    [Export(typeof(IClassifierProvider))]
    [ContentType(ChapelConstants.ChapelContentType)]
    public sealed class ChapelClassifierProvider : LanguageClassifierProvider<ChapelLanguagePackage>
    {
        protected override IClassifier GetClassifierImpl([NotNull] ITextBuffer textBuffer)
        {
            return new ChapelClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
