namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using IQuickInfoSession = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSession;
    using IQuickInfoSource = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSource;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;
    using ITextSnapshot = Microsoft.VisualStudio.Text.ITextSnapshot;
    using ITrackingSpan = Microsoft.VisualStudio.Text.ITrackingSpan;
    using SnapshotPoint = Microsoft.VisualStudio.Text.SnapshotPoint;

    internal class AlloyQuickInfoSource : IQuickInfoSource
    {
        private readonly ITextBuffer _textBuffer;

        public AlloyQuickInfoSource(ITextBuffer textBuffer)
        {
            _textBuffer = textBuffer;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
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
            }
        }

        public void Dispose()
        {
        }
    }
}
