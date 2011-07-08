namespace Tvl.VisualStudio.Language.Java
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Text;

    //[Export(typeof(IQuickInfoSourceProvider))]
    [ContentType(Constants.JavaContentType)]
    [Name("Java Tool Tip Quick Info Source")]
    public sealed class JavaQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new JavaQuickInfoSource(textBuffer);
        }
    }
}
