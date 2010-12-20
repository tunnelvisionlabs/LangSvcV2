namespace Tvl.VisualStudio.Text.Tagging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;

    public class BackgroundParserErrorTagger : ITagger<IErrorTag>
    {
        private ITagSpan<IErrorTag>[] _tags;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public BackgroundParserErrorTagger(ITextBuffer textBuffer, IBackgroundParser backgroundParser)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (backgroundParser == null)
                throw new ArgumentNullException("backgroundParser");
            Contract.EndContractBlock();

            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;
            this.BackgroundParser.ParseComplete += new EventHandler<ParseResultEventArgs>(BackgroundParserParseComplete);
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
        }

        public IBackgroundParser BackgroundParser
        {
            get;
            private set;
        }

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _tags;
        }

        private void BackgroundParserParseComplete(object sender, ParseResultEventArgs e)
        {
            var snapshot = TextBuffer.CurrentSnapshot;
            _tags = e.Errors.Select(error => new TagSpan<IErrorTag>(new SnapshotSpan(e.Snapshot, error.Span).TranslateTo(snapshot, SpanTrackingMode.EdgeExclusive), new ErrorTag(PredefinedErrorTypeNames.SyntaxError, error.Message))).ToArray();
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
