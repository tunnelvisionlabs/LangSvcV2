namespace Tvl.VisualStudio.Text
{
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Language.Intellisense;

    public interface ICompletionTarget
    {
        ITextView TextView
        {
            get;
        }

        IIntellisenseSessionStack IntellisenseSessionStack
        {
            get;
        }

        ICompletionBroker CompletionBroker
        {
            get;
        }

        IQuickInfoBroker QuickInfoBroker
        {
            get;
        }

        ISignatureHelpBroker SignatureHelpBroker
        {
            get;
        }

        CompletionInfo CompletionInfo
        {
            get;
        }

        ICompletionSession CompletionSession
        {
            get;
        }

        IQuickInfoSession QuickInfoSession
        {
            get;
        }

        ISignatureHelpSession SignatureHelpSession
        {
            get;
        }

        void CommitCompletion();

        void DismissCompletion();

        void DismissQuickInfo();

        void DismissSignatureHelp();

        void DismissAll();

        void TriggerCompletion(ITrackingPoint point);

        void TriggerQuickInfo(ITrackingPoint point);

        void TriggerSignatureHelp(ITrackingPoint point);
    }
}
