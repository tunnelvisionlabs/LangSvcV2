namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(IKeyProcessorProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Name("AlloyIntellisenseKeyPreprocessor")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Order(After = "VisualStudioKeyProcessor")]
    internal class AlloyIntellisenseKeyPostprocessorProvider : IntellisenseKeyPostprocessorProvider
    {
    }
}
