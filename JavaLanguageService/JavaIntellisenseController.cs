namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using System.Runtime.InteropServices;
    using System.ComponentModel;

    internal sealed class JavaIntellisenseController : IIntellisenseController
    {
        private readonly IQuickInfoBroker QuickInfoBroker;
        private readonly ICompletionBroker CompletionBroker;
        private readonly ISignatureHelpBroker SignatureHelpBroker;
        private ITextView textView;
        private IList<ITextBuffer> subjectBuffers;

        public JavaIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers, IQuickInfoBroker QuickInfoBroker, ICompletionBroker CompletionBroker, ISignatureHelpBroker SignatureHelpBroker)
        {
            this.textView = textView;
            this.subjectBuffers = subjectBuffers;
            this.QuickInfoBroker = QuickInfoBroker;
            this.CompletionBroker = CompletionBroker;
            this.SignatureHelpBroker = SignatureHelpBroker;
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void Detach(ITextView textView)
        {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }
    }
}
