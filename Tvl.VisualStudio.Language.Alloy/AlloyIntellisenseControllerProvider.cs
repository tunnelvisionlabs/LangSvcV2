namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.VisualStudio.Language.Intellisense;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text;

    [Export(typeof(IIntellisenseControllerProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Order]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("AlloyIntellisenseController")]
    internal class AlloyIntellisenseControllerProvider : IntellisenseControllerProvider
    {
        protected override IntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new AlloyIntellisenseController2(textView, this);
        }
    }
}
