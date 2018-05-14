namespace Tvl.VisualStudio.Text.Tagging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;

    public class BackgroundParserErrorTagger : ITagger<IErrorTag>
    {
        private readonly ITextBuffer _textBuffer;
        private readonly IBackgroundParser _backgroundParser;

        private ITagSpan<IErrorTag>[] _tags;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public BackgroundParserErrorTagger([NotNull] ITextBuffer textBuffer, [NotNull] IBackgroundParser backgroundParser)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));
            Requires.NotNull(backgroundParser, nameof(backgroundParser));

            this._textBuffer = textBuffer;
            this._backgroundParser = backgroundParser;
            this._backgroundParser.ParseComplete += HandleBackgroundParserParseComplete;
            this._backgroundParser.RequestParse(false);
        }

        [NotNull]
        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        [NotNull]
        public IBackgroundParser BackgroundParser
        {
            get
            {
                return _backgroundParser;
            }
        }

        [NotNull]
        public IEnumerable<ITagSpan<IErrorTag>> GetTags([NotNull] NormalizedSnapshotSpanCollection spans)
        {
            Requires.NotNull(spans, nameof(spans));

            return _tags;
        }

        IEnumerable<ITagSpan<IErrorTag>> ITagger<IErrorTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return GetTags(spans);
        }

        private void HandleBackgroundParserParseComplete(object sender, ParseResultEventArgs e)
        {
            var snapshot = TextBuffer.CurrentSnapshot;
            List<TagSpan<IErrorTag>> tags = new List<TagSpan<IErrorTag>>();
            foreach (var error in e.Errors)
            {
                try
                {
                    tags.Add(new TagSpan<IErrorTag>(new SnapshotSpan(e.Snapshot, error.Span).TranslateTo(snapshot, SpanTrackingMode.EdgeExclusive), new ErrorTag(PredefinedErrorTypeNames.SyntaxError, error.Message)));
                }
                catch (Exception ex) when (!ErrorHandler.IsCriticalException(ex))
                {
                }
            }
            _tags = tags.ToArray();
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            Debug.Assert(e != null);

            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
