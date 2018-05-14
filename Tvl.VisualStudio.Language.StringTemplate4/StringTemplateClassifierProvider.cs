namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Classification;

    [Export(typeof(IClassifierProvider))]
    [ContentType(StringTemplateConstants.StringTemplateContentType)]
    public sealed class StringTemplateClassifierProvider : LanguageClassifierProvider<StringTemplateLanguagePackage>
    {
        protected override IClassifier GetClassifierImpl([NotNull] ITextBuffer textBuffer)
        {
            return new StringTemplateClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
