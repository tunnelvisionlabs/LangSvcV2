namespace Tvl.VisualStudio.Text
{
    using System;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    public class CompletionTarget : ICompletionTarget
    {
        private readonly ITextView _textView;
        private readonly IIntellisenseSessionStack _intellisenseSessionStack;
        private readonly ICompletionBroker _completionBroker;
        private readonly IQuickInfoBroker _quickInfoBroker;
        private readonly ISignatureHelpBroker _signatureHelpBroker;
        private Text.CompletionInfo _completionInfo;

        public CompletionTarget(ITextView textView, IIntellisenseSessionStack intellisenseSessionStack, ICompletionBroker completionBroker, IQuickInfoBroker quickInfoBroker, ISignatureHelpBroker signatureHelpBroker)
        {
            _textView = textView;
            _intellisenseSessionStack = intellisenseSessionStack;
            _completionBroker = completionBroker;
            _quickInfoBroker = quickInfoBroker;
            _signatureHelpBroker = signatureHelpBroker;
        }

        public ITextView TextView
        {
            get
            {
                return _textView;
            }
        }

        public IIntellisenseSessionStack IntellisenseSessionStack
        {
            get
            {
                return _intellisenseSessionStack;
            }
        }

        public ICompletionBroker CompletionBroker
        {
            get
            {
                return _completionBroker;
            }
        }

        public IQuickInfoBroker QuickInfoBroker
        {
            get
            {
                return _quickInfoBroker;
            }
        }

        public ISignatureHelpBroker SignatureHelpBroker
        {
            get
            {
                return _signatureHelpBroker;
            }
        }

        public virtual CompletionInfo CompletionInfo
        {
            get
            {
                if (_completionInfo == null)
                    _completionInfo = CreateCompletionInfo();

                return _completionInfo;
            }
        }

        public ICompletionSession CompletionSession
        {
            get;
            protected set;
        }

        public IQuickInfoSession QuickInfoSession
        {
            get;
            protected set;
        }

        public ISignatureHelpSession SignatureHelpSession
        {
            get;
            protected set;
        }

        public virtual void CommitCompletion()
        {
            ICompletionSession session = CompletionSession;
            if (session != null && session.SelectedCompletionSet != null && session.SelectedCompletionSet.SelectionStatus != null)
            {
                Completion completion = session.SelectedCompletionSet.SelectionStatus.Completion;
                if (completion != null && session.SelectedCompletionSet.SelectionStatus.IsSelected)
                {
                    ITrackingSpan applicableToSpan = session.SelectedCompletionSet.ApplicableTo;
                    if (applicableToSpan != null && applicableToSpan.GetSpan(applicableToSpan.TextBuffer.CurrentSnapshot).GetText() != completion.InsertionText)
                        session.Commit();
                }
            }

            DismissCompletion();
        }

        public virtual void DismissCompletion()
        {
            ICompletionSession session = CompletionSession;
            CompletionSession = null;
            if (session != null && !session.IsDismissed)
                session.Dismiss();
        }

        public virtual void DismissQuickInfo()
        {
            IQuickInfoSession session = QuickInfoSession;
            QuickInfoSession = null;
            if (session != null && !session.IsDismissed)
                session.Dismiss();
        }

        public virtual void DismissSignatureHelp()
        {
            ISignatureHelpSession session = SignatureHelpSession;
            SignatureHelpSession = null;
            if (session != null && !session.IsDismissed)
                session.Dismiss();
        }

        public virtual void DismissAll()
        {
            DismissQuickInfo();
            DismissCompletion();
            DismissSignatureHelp();
        }

        public virtual void TriggerCompletion(ITrackingPoint point)
        {
            DismissCompletion();
            ICompletionSession session = CompletionBroker.TriggerCompletion(TextView, point, true);
            if (session != null)
            {
                session.Committed += HandleCompletionCommitted;
                session.Dismissed += HandleCompletionDismissed;
                CompletionSession = session;
            }
        }

        public virtual void TriggerQuickInfo(ITrackingPoint point)
        {
            DismissQuickInfo();

            IQuickInfoSession session;
            if (point != null)
                session = QuickInfoBroker.TriggerQuickInfo(TextView, point, true);
            else
                session = QuickInfoBroker.TriggerQuickInfo(TextView);

            if (session != null)
            {
                session.Dismissed += HandleQuickInfoDismissed;
                QuickInfoSession = session;
            }
        }

        public virtual void TriggerSignatureHelp(ITrackingPoint point)
        {
            DismissSignatureHelp();
            ISignatureHelpSession session = SignatureHelpBroker.TriggerSignatureHelp(TextView, point, true);
            if (session != null)
            {
                session.Dismissed += HandleSignatureHelpDismissed;
                SignatureHelpSession = session;
            }
        }

        protected virtual CompletionInfo CreateCompletionInfo()
        {
            return new CompletionInfo(this);
        }

        protected virtual void HandleCompletionCommitted(object sender, EventArgs e)
        {
            ITextView textView = TextView;
            if (textView != null)
            {
                IWpfTextView wpfTextView = textView as IWpfTextView;
                if (wpfTextView != null)
                    wpfTextView.VisualElement.Focus();

                textView.Caret.EnsureVisible();
            }
        }

        protected virtual void HandleCompletionDismissed(object sender, EventArgs e)
        {
            CompletionSession = null;
        }

        protected virtual void HandleQuickInfoDismissed(object sender, EventArgs e)
        {
            QuickInfoSession = null;
        }

        protected virtual void HandleSignatureHelpDismissed(object sender, EventArgs e)
        {
            SignatureHelpSession = null;
        }
    }
}
