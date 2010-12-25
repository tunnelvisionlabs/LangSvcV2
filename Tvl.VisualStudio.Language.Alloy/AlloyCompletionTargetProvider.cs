namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [ContentType(AlloyConstants.AlloyContentType)]
    [Export(typeof(ICompletionTargetProvider))]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class AlloyCompletionTargetProvider : CompletionTargetProvider
    {
    }
}
