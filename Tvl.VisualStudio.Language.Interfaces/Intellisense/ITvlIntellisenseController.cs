namespace Tvl.VisualStudio.Language.Intellisense
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Guid = System.Guid;
    using ICompletionSession = Microsoft.VisualStudio.Language.Intellisense.ICompletionSession;
    using IIntellisenseController = Microsoft.VisualStudio.Language.Intellisense.IIntellisenseController;
    using IIntellisenseSessionStack = Microsoft.VisualStudio.Language.Intellisense.IIntellisenseSessionStack;
    using IMouseProcessor = Microsoft.VisualStudio.Text.Editor.IMouseProcessor;
    using IntPtr = System.IntPtr;
    using IQuickInfoSession = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSession;
    using ISignatureHelpSession = Microsoft.VisualStudio.Language.Intellisense.ISignatureHelpSession;
    using ITextView = Microsoft.VisualStudio.Text.Editor.ITextView;
    using ITrackingPoint = Microsoft.VisualStudio.Text.ITrackingPoint;
    using OLECMDEXECOPT = Microsoft.VisualStudio.OLE.Interop.OLECMDEXECOPT;
    using SmartTagState = Microsoft.VisualStudio.Language.Intellisense.SmartTagState;
    using SmartTagType = Microsoft.VisualStudio.Language.Intellisense.SmartTagType;
    using VSOBJGOTOSRCTYPE = Microsoft.VisualStudio.Shell.Interop.VSOBJGOTOSRCTYPE;

    public interface ITvlIntellisenseController : IIntellisenseController
    {
        IIntellisenseSessionStack IntellisenseSessionStack
        {
            get;
        }

        ITextView TextView
        {
            get;
        }

        bool SupportsCommenting
        {
            get;
        }

        bool SupportsFormatting
        {
            get;
        }

        bool SupportsCompletion
        {
            get;
        }

        bool SupportsSignatureHelp
        {
            get;
        }

        bool SupportsQuickInfo
        {
            get;
        }

        bool SupportsGotoDefinition
        {
            get;
        }

        bool SupportsGotoDeclaration
        {
            get;
        }

        bool SupportsGotoReference
        {
            get;
        }

        IMouseProcessor CustomMouseProcessor
        {
            get;
        }

        ICompletionSession CompletionSession
        {
            get;
        }

        ISignatureHelpSession SignatureHelpSession
        {
            get;
        }

        IQuickInfoSession QuickInfoSession
        {
            get;
        }

        bool IsCompletionActive
        {
            get;
        }

        void GoToSource(VSOBJGOTOSRCTYPE gotoSourceType, [NotNull] ITrackingPoint triggerPoint);

        [NotNull]
        Task<IEnumerable<INavigateToTarget>> GoToSourceAsync(VSOBJGOTOSRCTYPE gotoSourceType, [NotNull] ITrackingPoint triggerPoint);

        [NotNull]
        IEnumerable<INavigateToTarget> GoToSourceImpl(VSOBJGOTOSRCTYPE gotoSourceType, [NotNull] ITrackingPoint triggerPoint);

        void TriggerCompletion([NotNull] ITrackingPoint triggerPoint);

        void TriggerCompletion([NotNull] ITrackingPoint triggerPoint, CompletionInfoType completionInfoType, IntellisenseInvocationType intellisenseInvocationType);

        void TriggerSignatureHelp([NotNull] ITrackingPoint triggerPoint);

        void TriggerQuickInfo([NotNull] ITrackingPoint triggerPoint);

        void DismissCompletion();

        void DismissQuickInfo();

        void DismissSignatureHelp();

        void DismissAll();

        bool PreprocessCommand(ref Guid commandGroup, uint commandId, OLECMDEXECOPT executionOptions, IntPtr pvaIn, IntPtr pvaOut);

        void PostprocessCommand();

        bool IsCommitChar(char c);

        bool CommitCompletion();
    }
}
