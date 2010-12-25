namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Language.Intellisense;

    public static class CompletionHelper
    {
        public static bool DoCallMatch(CompletionInfo completionInfo)
        {
            ICompletionSession session = completionInfo.CompletionTarget.CompletionSession;
            if (session != null)
            {
                ITextSnapshot snapshot = session.TextView.TextSnapshot;
                string text = snapshot.GetText(completionInfo.ApplicableTo.GetSpan(snapshot));
                if (string.IsNullOrEmpty(text))
                    return false;

                session.Match();
                CompletionSet set1;
                CompletionSet set2;
                CompletionSet set3;
                CompletionSet set4;
                foreach (CompletionSet set in session.CompletionSets.Where(i => i != null && i.SelectionStatus != null && i.SelectionStatus.Completion != null))
                {
                    throw new NotImplementedException();
                }
            }
            throw new NotImplementedException();
        }

        public static void DoTriggerCompletion(ICompletionTarget completionTarget, CompletionInfoType infoType, bool signatureHelpOnly, IntellisenseInvocationType invocationType)
        {
            var completionInfo = completionTarget.CompletionInfo;
            ITextView textView = completionTarget.TextView;
            SnapshotPoint? point = textView.Caret.Position.Point.GetPoint(textView.TextBuffer, PositionAffinity.Predecessor);
            if (point.HasValue)
            {
                completionInfo.InfoType = infoType;
                completionInfo.InvocationType = invocationType;
                ITrackingPoint trackingPoint = textView.TextBuffer.CurrentSnapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);
                if (!signatureHelpOnly)
                {
                    completionTarget.TriggerCompletion(trackingPoint);
                    DoCallMatch(completionInfo);
                }

                if (signatureHelpOnly || (completionInfo.CompletionFlags & CompletionFlags.HasParameterInfo) != 0)
                {
                    completionTarget.TriggerSignatureHelp(trackingPoint);
                }

                if (completionTarget.CompletionSession != null)
                {
                    completionTarget.IntellisenseSessionStack.MoveSessionToTop(completionTarget.CompletionSession);
                }
            }
        }

        public static bool IsCompletionPresenterActive(ICompletionTarget target, bool evenIfUsingDefaultPresenter)
        {
            if (target.CompletionBroker == null || target.CompletionSession == null || target.CompletionSession.IsDismissed)
                return false;

            //if (!evenIfUsingDefaultPresenter && HostableEditor._useDefaultPresenter)
            //    return false;

            if (target.CompletionBroker.IsCompletionActive(target.TextView))
                return true;

            if (target.SignatureHelpBroker == null)
                return false;

            return target.SignatureHelpBroker.IsSignatureHelpActive(target.TextView);
        }
    }
}
