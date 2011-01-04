namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using IQuickInfoSession = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSession;
    using IQuickInfoSource = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSource;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;
    using ITextSnapshot = Microsoft.VisualStudio.Text.ITextSnapshot;
    using ITrackingSpan = Microsoft.VisualStudio.Text.ITrackingSpan;
    using SnapshotPoint = Microsoft.VisualStudio.Text.SnapshotPoint;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Alloy.IntellisenseModel;
    using System;
    using Microsoft.VisualStudio;

    internal class AlloyQuickInfoSource : IQuickInfoSource
    {
        private readonly ITextBuffer _textBuffer;
        private readonly AlloyQuickInfoSourceProvider _provider;

        public AlloyQuickInfoSource(ITextBuffer textBuffer, AlloyQuickInfoSourceProvider provider)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (provider == null)
                throw new ArgumentNullException("provider");

            _textBuffer = textBuffer;
            _provider = provider;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        public AlloyQuickInfoSourceProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;
            if (session == null || quickInfoContent == null)
                return;

            if (session.TextView.TextBuffer == this.TextBuffer)
            {
                ITextSnapshot currentSnapshot = this.TextBuffer.CurrentSnapshot;
                SnapshotPoint? triggerPoint = session.GetTriggerPoint(currentSnapshot);
                if (!triggerPoint.HasValue)
                    return;

                var selection = session.TextView.Selection.StreamSelectionSpan;
                if (!selection.IsEmpty && selection.Contains(new VirtualSnapshotPoint(triggerPoint.Value)))
                {
                    applicableToSpan = selection.Snapshot.CreateTrackingSpan(selection.SnapshotSpan, SpanTrackingMode.EdgeExclusive);
                    try
                    {
                        Expression currentExpression = Provider.IntellisenseCache.ParseExpression(selection);
                        if (currentExpression != null)
                        {
                            SnapshotSpan? span = currentExpression.Span;
                            if (span.HasValue)
                                applicableToSpan = span.Value.Snapshot.CreateTrackingSpan(span.Value, SpanTrackingMode.EdgeExclusive);

                            quickInfoContent.Add(currentExpression.ToString());
                        }
                        else
                        {
                            quickInfoContent.Add("Could not parse expression.");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ErrorHandler.IsCriticalException(ex))
                            throw;

                        quickInfoContent.Add(ex.Message);
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
