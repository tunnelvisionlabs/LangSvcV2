namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.VisualStudio.Text;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Text.Editor;

    [Export(typeof(IKeyProcessorProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Name("AlloyIntellisenseKeyPreprocessor")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Order(After = "AlloyIntellisenseKeyPreprocessor")]
    internal class AlloyIntellisenseKeyPostprocessorProvider : IntellisenseKeyPostprocessorProvider
    {
    }
}
