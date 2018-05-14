namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;

    public class Commenter : ICommenter
    {
        private readonly ITextView _textView;
        private readonly ITextUndoHistoryRegistry _textUndoHistoryRegistry;
        private readonly ReadOnlyCollection<CommentFormat> _commentFormats;
        private readonly ReadOnlyCollection<BlockCommentFormat> _blockFormats;
        private readonly ReadOnlyCollection<LineCommentFormat> _lineFormats;
        private readonly bool _useLineComments;

        public Commenter([NotNull] ITextView textView, [NotNull] ITextUndoHistoryRegistry textUndoHistoryRegistry, params CommentFormat[] commentFormats)
            : this(textView, textUndoHistoryRegistry, commentFormats.AsEnumerable())
        {
            Debug.Assert(textView != null);
            Debug.Assert(textUndoHistoryRegistry != null);
        }

        public Commenter([NotNull] ITextView textView, [NotNull] ITextUndoHistoryRegistry textUndoHistoryRegistry, [NotNull] IEnumerable<CommentFormat> commentFormats)
        {
            Requires.NotNull(textView, nameof(textView));
            Requires.NotNull(textUndoHistoryRegistry, nameof(textUndoHistoryRegistry));
            Requires.NotNull(commentFormats, nameof(commentFormats));

            this._textView = textView;
            this._textUndoHistoryRegistry = textUndoHistoryRegistry;
            this._commentFormats = commentFormats.ToList().AsReadOnly();
            this._blockFormats = _commentFormats.OfType<BlockCommentFormat>().ToList().AsReadOnly();
            this._lineFormats = _commentFormats.OfType<LineCommentFormat>().ToList().AsReadOnly();
            this._useLineComments = this._lineFormats.Count > 0;
        }

        [NotNull]
        public ITextView TextView
        {
            get
            {
                return _textView;
            }
        }

        [NotNull]
        public ITextUndoHistoryRegistry TextUndoHistoryRegistry
        {
            get
            {
                return _textUndoHistoryRegistry;
            }
        }

        [NotNull]
        public virtual ReadOnlyCollection<CommentFormat> CommentFormats
        {
            get
            {
                return _commentFormats;
            }
        }

        [NotNull]
        public virtual ReadOnlyCollection<BlockCommentFormat> BlockFormats
        {
            get
            {
                return _blockFormats;
            }
        }

        public virtual BlockCommentFormat PreferredBlockFormat
        {
            get
            {
                var formats = BlockFormats;
                if (formats == null || formats.Count == 0)
                    return null;

                return formats[0];
            }
        }

        [NotNull]
        public virtual ReadOnlyCollection<LineCommentFormat> LineFormats
        {
            get
            {
                return _lineFormats;
            }
        }

        public virtual LineCommentFormat PreferredLineFormat
        {
            get
            {
                var formats = LineFormats;
                if (formats == null || formats.Count == 0)
                    return null;

                return formats[0];
            }
        }

        public virtual bool UseLineComments
        {
            get
            {
                return _useLineComments;
            }
        }

        [NotNull]
        public virtual NormalizedSnapshotSpanCollection CommentSpans([NotNull] NormalizedSnapshotSpanCollection spans)
        {
            Requires.NotNull(spans, nameof(spans));

            List<SnapshotSpan> result = new List<SnapshotSpan>();

            if (spans.Count == 0)
                return new NormalizedSnapshotSpanCollection();

            var undoHistory = TextUndoHistoryRegistry.RegisterHistory(TextView);
            using (var transaction = undoHistory.CreateTransaction("Comment Selection"))
            {
                ITextSnapshot snapshot = spans[0].Snapshot;

                using (var edit = snapshot.TextBuffer.CreateEdit())
                {
                    foreach (var span in spans)
                    {
                        var selection = CommentSpan(span, edit);
                        result.Add(selection);
                    }

                    edit.Apply();
                }

                if (snapshot != TextView.TextSnapshot)
                    transaction.Complete();
            }

            if (result.Count > 1)
                result.RemoveAll(span => span.IsEmpty);

            var target = TextView.TextBuffer.CurrentSnapshot;
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = result[i].TranslateTo(target, SpanTrackingMode.EdgeInclusive);
            }

            return new NormalizedSnapshotSpanCollection(result);
        }

        [NotNull]
        public virtual NormalizedSnapshotSpanCollection UncommentSpans([NotNull] NormalizedSnapshotSpanCollection spans)
        {
            Requires.NotNull(spans, nameof(spans));

            List<SnapshotSpan> result = new List<SnapshotSpan>();

            if (spans.Count == 0)
                return new NormalizedSnapshotSpanCollection();

            var undoHistory = TextUndoHistoryRegistry.RegisterHistory(TextView);
            using (var transaction = undoHistory.CreateTransaction("Uncomment Selection"))
            {
                ITextSnapshot snapshot = spans[0].Snapshot;

                using (var edit = snapshot.TextBuffer.CreateEdit())
                {
                    foreach (var span in spans)
                    {
                        var selection = UncommentSpan(span, edit);
                        result.Add(selection);
                    }

                    edit.Apply();
                }

                if (snapshot != TextView.TextSnapshot)
                    transaction.Complete();
            }

            if (result.Count > 1)
                result.RemoveAll(span => span.IsEmpty);

            var target = TextView.TextBuffer.CurrentSnapshot;
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = result[i].TranslateTo(target, SpanTrackingMode.EdgeInclusive);
            }

            return new NormalizedSnapshotSpanCollection(result);
        }

        protected virtual SnapshotSpan CommentSpan(SnapshotSpan span, [NotNull] ITextEdit edit)
        {
            Requires.NotNull(edit, nameof(edit));

            span = span.TranslateTo(edit.Snapshot, SpanTrackingMode.EdgeExclusive);

            /*
             * Use line comments if:
             *  UseLineComments is true
             *  AND one of the following is true:
             *
             *  1. there is no selected text
             *  2. on the line where the selection starts, there is only whitespace up to the selection start point
             *     AND on the line where the selection ends, there is only whitespace up to the selection end point,
             *         OR there is only whitespace from the selection end point to the end of the line
             *
             * Use block comments if:
             *  We are not using line comments
             *  AND some text is selected
             *  AND PreferredBlockFormat is not null
             */
            var startContainingLine = span.Start.GetContainingLine();
            var endContainingLine = span.End.GetContainingLine();

            if (UseLineComments
                && (span.IsEmpty ||
                    (string.IsNullOrWhiteSpace(startContainingLine.GetText().Substring(0, span.Start - startContainingLine.Start))
                        && (string.IsNullOrWhiteSpace(endContainingLine.GetText().Substring(0, span.End - endContainingLine.Start))
                            || string.IsNullOrWhiteSpace(endContainingLine.GetText().Substring(span.End - endContainingLine.Start)))
                   )))
            {
                span = CommentLines(span, edit, PreferredLineFormat);
            }
            else if (
                span.Length > 0
                && PreferredBlockFormat != null
                )
            {
                span = CommentBlock(span, edit, PreferredBlockFormat);
            }

            return span;
        }

        protected virtual SnapshotSpan CommentLines(SnapshotSpan span, [NotNull] ITextEdit edit, [NotNull] LineCommentFormat format)
        {
            Requires.NotNull(edit, nameof(edit));
            Requires.NotNull(format, nameof(format));

            /*
             * Rules for line comments:
             *  Make sure line comments are indented as far as possible, skipping empty lines as necessary
             *  Don't comment N+1 lines when only N lines were selected my clicking in the left margin
             */
            if (span.End.GetContainingLine().LineNumber > span.Start.GetContainingLine().LineNumber && span.End.GetContainingLine().Start == span.End)
            {
                SnapshotPoint start = span.Start;
                SnapshotPoint end = span.Snapshot.GetLineFromLineNumber(span.End.GetContainingLine().LineNumber - 1).Start;
                if (end < start)
                    start = end;

                span = new SnapshotSpan(start, end);
            }

            int minindex = (from i in Enumerable.Range(span.Start.GetContainingLine().LineNumber, span.End.GetContainingLine().LineNumber - span.Start.GetContainingLine().LineNumber + 1)
                            where span.Snapshot.GetLineFromLineNumber(i).GetText().Trim().Length > 0
                            select ScanToNonWhitespaceChar(span.Snapshot.GetLineFromLineNumber(i)))
                           .Min();

            //comment each line
            for (int line = span.Start.GetContainingLine().LineNumber; line <= span.End.GetContainingLine().LineNumber; line++)
            {
                if (span.Snapshot.GetLineFromLineNumber(line).GetText().Trim().Length > 0)
                    edit.Insert(span.Snapshot.GetLineFromLineNumber(line).Start + minindex, format.StartText);
            }

            span = new SnapshotSpan(span.Start.GetContainingLine().Start, span.End.GetContainingLine().End);
            return span;
        }

        protected virtual SnapshotSpan CommentBlock(SnapshotSpan span, [NotNull] ITextEdit edit, [NotNull] BlockCommentFormat format)
        {
            Requires.NotNull(edit, nameof(edit));
            Requires.NotNull(format, nameof(format));

            //sp. case no selection
            if (span.IsEmpty)
            {
                span = new SnapshotSpan(span.Start.GetContainingLine().Start + ScanToNonWhitespaceChar(span.Start.GetContainingLine()), span.End.GetContainingLine().End);
            }

            // add start comment
            edit.Insert(span.Start, format.StartText);
            // add end comment
            edit.Insert(span.End, format.EndText);

            return span;
        }

        protected virtual SnapshotSpan UncommentSpan(SnapshotSpan span, [NotNull] ITextEdit edit)
        {
            Requires.NotNull(edit, nameof(edit));

            span = span.TranslateTo(edit.Snapshot, SpanTrackingMode.EdgeExclusive);
            bool useLineComments = true;
            var startContainingLine = span.Start.GetContainingLine();
            var endContainingLine = span.End.GetContainingLine();

            // special case: empty span
            if (span.IsEmpty)
            {
                if (useLineComments)
                    span = UncommentLines(span, edit, LineFormats);
            }
            else
            {
                SnapshotSpan resultSpan;
                if (TryUncommentBlock(span, edit, BlockFormats, out resultSpan))
                    return resultSpan;

                if (useLineComments)
                {
                    span = UncommentLines(span, edit, LineFormats);
                }
            }

            return span;
        }

        protected virtual SnapshotSpan UncommentLines(SnapshotSpan span, [NotNull] ITextEdit edit, [NotNull, ItemNotNull] ReadOnlyCollection<LineCommentFormat> formats)
        {
            Requires.NotNull(edit, nameof(edit));
            Requires.NotNullOrNullElements(formats, nameof(formats));

            if (span.End.GetContainingLine().LineNumber > span.Start.GetContainingLine().LineNumber && span.End == span.End.GetContainingLine().Start)
            {
                SnapshotPoint start = span.Start;
                SnapshotPoint end = span.Snapshot.GetLineFromLineNumber(span.End.GetContainingLine().LineNumber - 1).Start;
                if (end < start)
                    start = end;

                span = new SnapshotSpan(start, end);
            }

            // Remove line comments
            for (int line = span.Start.GetContainingLine().LineNumber; line <= span.End.GetContainingLine().LineNumber; line++)
            {
                int i = ScanToNonWhitespaceChar(span.Snapshot.GetLineFromLineNumber(line));
                string text = span.Snapshot.GetLineFromLineNumber(line).GetText();
                foreach (var format in formats)
                {
                    int clen = format.StartText.Length;
                    if ((text.Length > i + clen) && text.Substring(i, clen) == format.StartText)
                    {
                        // remove line comment.
                        edit.Delete(span.Snapshot.GetLineFromLineNumber(line).Start.Position + i, clen);
                        break;
                    }
                }
            }

            span = new SnapshotSpan(span.Start.GetContainingLine().Start, span.End.GetContainingLine().End);
            return span;
        }

        protected virtual bool TryUncommentBlock(SnapshotSpan span, [NotNull] ITextEdit edit, [NotNull, ItemNotNull] ReadOnlyCollection<BlockCommentFormat> formats, out SnapshotSpan result)
        {
            Requires.NotNull(edit, nameof(edit));
            Requires.NotNullOrNullElements(formats, nameof(formats));

            foreach (var format in formats)
            {
                if (TryUncommentBlock(span, edit, format, out result))
                    return true;
            }

            result = default(SnapshotSpan);
            return false;
        }

        protected virtual bool TryUncommentBlock(SnapshotSpan span, [NotNull] ITextEdit edit, [NotNull] BlockCommentFormat format, out SnapshotSpan result)
        {
            Requires.NotNull(edit, nameof(edit));
            Requires.NotNull(format, nameof(format));

            string blockStart = format.StartText;
            string blockEnd = format.EndText;

            int startLen = span.Start.GetContainingLine().Length;
            int endLen = span.End.GetContainingLine().Length;

            TrimSpan(ref span);

            //sp. case no selection, try and uncomment the current line.
            if (span.IsEmpty)
            {
                span = new SnapshotSpan(span.Start.GetContainingLine().Start + ScanToNonWhitespaceChar(span.Start.GetContainingLine()), span.End.GetContainingLine().End);
            }

            // Check that comment start and end blocks are possible.
            if ((span.Start - span.Start.GetContainingLine().Start) + blockStart.Length <= startLen && (span.End - span.End.GetContainingLine().Start) - blockStart.Length >= 0)
            {
                string startText = span.Snapshot.GetText(span.Start.Position, blockStart.Length);

                if (startText == blockStart)
                {
                    SnapshotSpan linespan = span;
                    linespan = new SnapshotSpan(span.End - blockEnd.Length, span.End);
                    string endText = linespan.GetText();
                    if (endText == blockEnd)
                    {
                        //yes, block comment selected; remove it
                        edit.Delete(linespan);
                        edit.Delete(span.Start.Position, blockStart.Length);
                        result = span;
                        return true;
                    }
                }
            }

            result = default(SnapshotSpan);
            return false;
        }

        protected static void TrimSpan(ref SnapshotSpan span)
        {
            string text = span.GetText();
            int length = text.Trim().Length;

            int offset = 0;
            while (offset < text.Length && char.IsWhiteSpace(text[offset]))
                offset++;

            if (offset > 0 || length != text.Length)
                span = new SnapshotSpan(span.Start + offset, length);
        }

        protected static int ScanToNonWhitespaceChar([NotNull] ITextSnapshotLine line)
        {
            Requires.NotNull(line, nameof(line));

            string text = line.GetText();
            int len = text.Length;
            int i = 0;
            while (i < len && char.IsWhiteSpace(text[i]))
            {
                i++;
            }
            return i;
        }
    }
}
