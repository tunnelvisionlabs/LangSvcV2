namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    [Name("AlloyCompletionSourceProvider")]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Order(Before = "default")]
    [Export(typeof(ICompletionSourceProvider))]
    internal class AlloyCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new AlloyCompletionSource(textBuffer, this);
        }
    }
}
