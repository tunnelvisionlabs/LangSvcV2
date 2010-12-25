namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IKeyProcessorProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Name("AlloyIntellisenseKeyPreprocessor")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Order(Before = "AlloyIntellisenseKeyPostprocessor")]
    internal class AlloyIntellisenseKeyPreprocessorProvider : IntellisenseKeyPreprocessorProvider
    {
    }
}
