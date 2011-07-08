namespace Tvl.VisualStudio.Language.Java
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    //[Export(typeof(IIntellisenseControllerProvider))]
    [Name("Java IntelliSense Controller")]
    [ContentType(Constants.JavaContentType)]
    public sealed class JavaIntellisenseControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        private IQuickInfoBroker QuickInfoBroker
        {
            get;
            set;
        }

        [Import]
        private ICompletionBroker CompletionBroker
        {
            get;
            set;
        }

        [Import]
        private ISignatureHelpBroker SignatureHelpBroker
        {
            get;
            set;
        }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new JavaIntellisenseController(textView, subjectBuffers, QuickInfoBroker, CompletionBroker, SignatureHelpBroker);
        }
    }
}
