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
        public static bool DoCallMatch(ICompletionTarget completionTarget)
        {
            bool flag2 = false;
            ICompletionSession session = completionTarget.CompletionSession;
            if (session != null)
            {
                ITextSnapshot snapshot = session.TextView.TextSnapshot;
                string text = snapshot.GetText(completionTarget.CompletionInfo.ApplicableTo.GetSpan(snapshot));
                if (string.IsNullOrEmpty(text))
                    return false;

                session.Match();
                CompletionSet set1 = null;
                CompletionSet set2 = null;
                CompletionSet set3 = null;
                CompletionSet set4 = null;
                bool flag3 = false;
                bool flag4 = false;
                foreach (CompletionSet set in session.CompletionSets.Where(i => i != null && i.SelectionStatus != null && i.SelectionStatus.Completion != null))
                {
                    flag2 = true;
                    bool isAllTab = false;
                    if (isAllTab)
                    {
                        set3 = set;
                        flag3 = string.Equals(text, set.SelectionStatus.Completion.DisplayText, StringComparison.CurrentCultureIgnoreCase);
                    }
                    else
                    {
                        set4 = set;
                        flag4 = string.Equals(text, set.SelectionStatus.Completion.DisplayText, StringComparison.CurrentCultureIgnoreCase);
                    }
                }

                if (flag3 && !flag4)
                {
                    set1 = set3;
                }
                else if (set2 != null)
                {
                    if (set2 != set3 && set4 == null)
                        set1 = set3;
                }
                else if (set4 != null)
                {
                    set1 = set4;
                }
                else
                {
                    set1 = set3;
                }

                if (set1 != null)
                {
                    session.SelectedCompletionSet = set1;
                }
            }

            return flag2;
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
                    DoCallMatch(completionTarget);
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
