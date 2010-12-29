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
    [Order(Before = "VisualStudioKeyProcessor")]
    internal class AlloyIntellisenseKeyPreprocessorProvider : IntellisenseKeyPreprocessorProvider
    {
    }
}
