namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Classification;
    using AntlrLanguagePackage = Tvl.VisualStudio.Language.Antlr3.AntlrLanguagePackage;

    [Export(typeof(IClassifierProvider))]
    [ContentType(Antlr4Constants.AntlrContentType)]
    public sealed class Antlr4ClassifierProvider : LanguageClassifierProvider<AntlrLanguagePackage>
    {
        protected override IClassifier GetClassifierImpl([NotNull] ITextBuffer textBuffer)
        {
            return new Antlr4Classifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
