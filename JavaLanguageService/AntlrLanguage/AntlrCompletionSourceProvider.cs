namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using System.ComponentModel.Composition;

    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    [Name("ANTLR Completion Source")]
    [Order]
    internal sealed class AntlrCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            Func<AntlrCompletionSource> creator = () => new AntlrCompletionSource(textBuffer);
            return textBuffer.Properties.GetOrCreateSingletonProperty(creator);
        }
    }
}
