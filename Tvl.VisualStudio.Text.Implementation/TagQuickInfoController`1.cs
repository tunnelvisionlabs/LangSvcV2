namespace Tvl.VisualStudio.Text.Implementation
{
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;

    public class TagQuickInfoController<T> : IIntellisenseController
        where T : ITag
    {
        private IQuickInfoBroker _quickInfoBroker;
        private ITextView _textView;
        private ITagAggregator<T> _tagAggregator;
        private ITextBuffer _surfaceBuffer;
        private IQuickInfoSession _session;

        public TagQuickInfoController(IQuickInfoBroker quickInfoBroker, ITextView textView, ITagAggregator<T> tagAggregator)
        {
            this._quickInfoBroker = quickInfoBroker;
            this._textView = textView;
            this._tagAggregator = tagAggregator;

            this._surfaceBuffer = textView.TextViewModel.DataBuffer;
            this._surfaceBuffer.Changed += OnSurfaceBuffer_Changed;
            this._textView.MouseHover += OnTextView_MouseHover;
            this._textView.Caret.PositionChanged += OnCaret_PositionChanged;
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void Detach(ITextView textView)
        {
            this._surfaceBuffer.Changed -= OnSurfaceBuffer_Changed;
            this._textView.MouseHover -= OnTextView_MouseHover;
            this._textView.Caret.PositionChanged -= OnCaret_PositionChanged;
            if (this._tagAggregator != null)
            {
                this._tagAggregator.Dispose();
                this._tagAggregator = null;
            }
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        internal void DismissSession()
        {
            if ((this._session != null) && !this._session.IsDismissed)
            {
                this._session.Dismiss();
                this._session = null;
            }
        }

        private bool EnsureSessionStillValid(SnapshotPoint point)
        {
            if (this._session != null)
            {
                if (this._session.IsDismissed)
                {
                    this._session = null;
                    return false;
                }
                if ((this._session.ApplicableToSpan.TextBuffer == point.Snapshot.TextBuffer) && this._session.ApplicableToSpan.GetSpan(point.Snapshot).IntersectsWith(new Span(point.Position, 0)))
                {
                    return true;
                }
                this._session.Dismiss();
                this._session = null;
            }
            return false;
        }

        private bool TryExtractQuickInfoFromMarkers(int position)
        {
            IMappingTagSpan<T> mappingTagSpan = null;

            foreach (IMappingTagSpan<T> span in this._tagAggregator.GetTags(new SnapshotSpan(this._textView.TextSnapshot, position, 1)))
            {
                if (span.Tag != null)
                    mappingTagSpan = span;
            }

            if (mappingTagSpan != null)
            {
                NormalizedSnapshotSpanCollection spans = mappingTagSpan.Span.GetSpans(this._textView.TextBuffer);
                if (spans.Count > 0)
                {
                    this.DismissSession();
                    SnapshotSpan span = spans[0];
                    ITrackingPoint triggerPoint = span.Snapshot.CreateTrackingPoint(span.Start.Position, PointTrackingMode.Positive);
                    this._session = this._quickInfoBroker.CreateQuickInfoSession(this._textView, triggerPoint, true);
                    this._session.Start();
                    return true;
                }
            }

            return false;
        }

        private void OnSurfaceBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            this.DismissSession();
        }

        private void OnTextView_MouseHover(object sender, MouseHoverEventArgs e)
        {
            SnapshotPoint? nullable = e.TextPosition.GetPoint(this._surfaceBuffer, PositionAffinity.Successor);
            if (!nullable.HasValue)
                return;

            SnapshotPoint point = nullable.Value;
            if (this.EnsureSessionStillValid(point))
                return;

            if (this._tagAggregator != null)
            {
                this.TryExtractQuickInfoFromMarkers(e.Position);
            }
        }

        private void OnCaret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            this.DismissSession();
        }
    }
}
