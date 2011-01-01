namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Intellisense;

    [Export(typeof(IIntellisenseControllerProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Order]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("AlloyIntellisenseController")]
    internal class AlloyIntellisenseControllerProvider : IntellisenseControllerProvider
    {
        protected override IntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new AlloyIntellisenseController(textView, this);
        }
    }
}
