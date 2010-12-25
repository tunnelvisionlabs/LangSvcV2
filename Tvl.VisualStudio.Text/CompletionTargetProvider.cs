namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;

    public class CompletionTargetProvider : ICompletionTargetProvider
    {
        [Import]
        public IIntellisenseSessionStackMapService IntellisenseSessionStackMapService
        {
            get;
            private set;
        }

        [Import]
        public ICompletionBroker CompletionBroker
        {
            get;
            private set;
        }

        [Import]
        public IQuickInfoBroker QuickInfoBroker
        {
            get;
            private set;
        }

        [Import]
        public ISignatureHelpBroker SignatureHelpBroker
        {
            get;
            private set;
        }

        public virtual ICompletionTarget CreateCompletionTarget(ITextView textView)
        {
            return new CompletionTarget(textView, IntellisenseSessionStackMapService.GetStackForTextView(textView), CompletionBroker, QuickInfoBroker, SignatureHelpBroker);
        }
    }
}
