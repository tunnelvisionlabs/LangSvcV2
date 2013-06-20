namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    public class TokenCounter
    {
        private readonly ITextBuffer _textBuffer;
        private readonly ITagAggregator<IClassificationTag> _classifierAggregator;
        private readonly string _tokenText;
        private readonly TokenCountTree _tree;

        public TokenCounter(ITextBuffer textBuffer, IBufferTagAggregatorFactoryService bufferTagAggregatorFactoryService, string tokenText)
        {
            _textBuffer = textBuffer;
            _classifierAggregator = bufferTagAggregatorFactoryService.CreateTagAggregator<IClassificationTag>(textBuffer);
            _tokenText = tokenText;
            _tree = new TokenCountTree(this, _textBuffer.CurrentSnapshot);

            _classifierAggregator.BatchedTagsChanged += HandleClassifierTagsChanged;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        public ITagAggregator<IClassificationTag> ClassifierAggregator
        {
            get
            {
                return _classifierAggregator;
            }
        }

        public string TokenText
        {
            get
            {
                return _tokenText;
            }
        }

        private TokenCountTree Tree
        {
            get
            {
                return _tree;
            }
        }

        public int GetTokenCountAtStartOfLine(int line)
        {
            throw new NotImplementedException();
        }

        public NormalizedSnapshotSpanCollection GetTokenSpans()
        {
            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;
            return GetTokenSpans(new SnapshotSpan(snapshot, 0, snapshot.Length), SpanTrackingMode.EdgeInclusive);
        }

        public NormalizedSnapshotSpanCollection GetTokenSpans(SnapshotSpan span, SpanTrackingMode spanTrackingMode)
        {
            throw new NotImplementedException();
        }

        public SnapshotSpan? GetLastStartingBefore(SnapshotPoint point, PointTrackingMode pointTrackingMode)
        {
            throw new NotImplementedException();
        }

        public SnapshotSpan? GetLastEndingBefore(SnapshotPoint point, PointTrackingMode pointTrackingMode)
        {
            throw new NotImplementedException();
        }

        public SnapshotSpan? GetFirstEndingAfter(SnapshotPoint point, PointTrackingMode pointTrackingMode)
        {
            throw new NotImplementedException();
        }

        public SnapshotSpan? GetFirstStartingAfter(SnapshotPoint point, PointTrackingMode pointTrackingMode)
        {
            throw new NotImplementedException();
        }

        protected virtual void HandleClassifierTagsChanged(object sender, BatchedTagsChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
