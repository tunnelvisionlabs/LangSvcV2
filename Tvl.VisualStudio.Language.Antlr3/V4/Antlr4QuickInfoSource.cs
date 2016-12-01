namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Text.Navigation;
    using AntlrClassificationTypeNames = Tvl.VisualStudio.Language.Antlr3.AntlrClassificationTypeNames;

    internal sealed class Antlr4QuickInfoSource : IQuickInfoSource
    {
        public Antlr4QuickInfoSource(ITextBuffer textBuffer, IEditorNavigationSourceAggregator editorNavigationSourceAggregator, ITagAggregator<ClassificationTag> aggregator)
        {
            this.Aggregator = aggregator;
            this.EditorNavigationSourceAggregator = editorNavigationSourceAggregator;
            this.TextBuffer = textBuffer;
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
        }

        public IEditorNavigationSourceAggregator EditorNavigationSourceAggregator
        {
            get;
            private set;
        }

        public ITagAggregator<ClassificationTag> Aggregator
        {
            get;
            private set;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            if (session.TextView.TextBuffer == this.TextBuffer)
            {
                ITextSnapshot currentSnapshot = this.TextBuffer.CurrentSnapshot;
                SnapshotPoint? triggerPoint = session.GetTriggerPoint(currentSnapshot);
                if (!triggerPoint.HasValue)
                    return;

                foreach (var span in this.Aggregator.GetTags(new SnapshotSpan(triggerPoint.Value, triggerPoint.Value)))
                {
                    if (!span.Tag.ClassificationType.IsOfType(AntlrClassificationTypeNames.LexerRule)
                        && !span.Tag.ClassificationType.IsOfType(AntlrClassificationTypeNames.ParserRule))
                    {
                        continue;
                    }

                    NormalizedSnapshotSpanCollection spans = span.Span.GetSpans(currentSnapshot);
                    if (spans.Count != 1)
                        continue;

                    SnapshotSpan span2 = spans[0];
                    SnapshotSpan anchorSpan = span.Span.GetSpans(span.Span.AnchorBuffer)[0];
                    if (span2.Length != anchorSpan.Length)
                        continue;

                    if (!span2.Contains(triggerPoint.Value))
                        continue;

                    var rules = EditorNavigationSourceAggregator.GetNavigationTargets().ToArray();
                    if (rules.Length == 0)
                    {
                        quickInfoContent.Add("Parsing...");
                        applicableToSpan = currentSnapshot.CreateTrackingSpan(span2, SpanTrackingMode.EdgeExclusive);
                        return;
                    }

                    string ruleName = span2.GetText();
                    IEditorNavigationTarget target = null;
                    foreach (var rule in rules)
                    {
                        if (string.Equals(rule.Name, ruleName))
                        {
                            target = rule;
                            break;
                        }
                    }

                    if (target == null)
                        continue;

                    SnapshotSpan targetSpan = target.Span;
                    quickInfoContent.Add(!targetSpan.IsEmpty ? targetSpan.GetText() : target.Name);
                    applicableToSpan = currentSnapshot.CreateTrackingSpan(span2, SpanTrackingMode.EdgeExclusive);
                    return;
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                EditorNavigationSourceAggregator.Dispose();
            }
        }
    }
}
