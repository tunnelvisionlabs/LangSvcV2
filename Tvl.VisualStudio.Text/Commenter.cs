namespace Tvl.VisualStudio.Text
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;

    public class Commenter : ICommenter
    {
        public Commenter(ITextView textView, ITextUndoHistoryRegistry textUndoHistoryRegistry, CommentFormat commentFormat)
        {
            this.TextView = textView;
            this.TextUndoHistoryRegistry = textUndoHistoryRegistry;
            this.CommentFormat = commentFormat;
        }

        public ITextView TextView
        {
            get;
            private set;
        }

        public ITextUndoHistoryRegistry TextUndoHistoryRegistry
        {
            get;
            private set;
        }

        public CommentFormat CommentFormat
        {
            get;
            private set;
        }

        public virtual NormalizedSnapshotSpanCollection CommentSpans(NormalizedSnapshotSpanCollection spans)
        {
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

        public virtual NormalizedSnapshotSpanCollection UncommentSpans(NormalizedSnapshotSpanCollection spans)
        {
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

        protected virtual SnapshotSpan CommentSpan(SnapshotSpan span, ITextEdit edit)
        {
            span = span.TranslateTo(edit.Snapshot, SpanTrackingMode.EdgeExclusive);

            /*
             * Use line comments if:
             *  UseLineComments is true
             *  AND LineStart is not null or empty
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
             *  AND BlockStart is not null or empty
             *  AND BlockEnd is not null or empty
             */
            var startContainingLine = span.Start.GetContainingLine();
            var endContainingLine = span.End.GetContainingLine();

            if (CommentFormat.UseLineComments
                && !string.IsNullOrEmpty(CommentFormat.LineStart)
                && (span.IsEmpty ||
                    ((startContainingLine.GetText().Substring(0, span.Start - startContainingLine.Start).Trim().Length == 0)
                        && ((endContainingLine.GetText().Substring(0, span.End - endContainingLine.Start).Trim().Length == 0)
                            || (endContainingLine.GetText().Substring(span.End - endContainingLine.Start).Trim().Length == 0))
                   )))
            {
                span = CommentLines(span, edit, CommentFormat.LineStart);
            }
            else if (
                span.Length > 0
                && !string.IsNullOrEmpty(CommentFormat.BlockStart)
                && !string.IsNullOrEmpty(CommentFormat.BlockEnd)
                )
            {
                span = CommentBlock(span, edit, CommentFormat.BlockStart, CommentFormat.BlockEnd);
            }

            return span;
        }

        protected virtual SnapshotSpan CommentLines(SnapshotSpan span, ITextEdit edit, string lineComment)
        {
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
                    edit.Insert(span.Snapshot.GetLineFromLineNumber(line).Start + minindex, lineComment);
            }

            span = new SnapshotSpan(span.Start.GetContainingLine().Start, span.End.GetContainingLine().End);
            return span;
        }

        protected virtual SnapshotSpan CommentBlock(SnapshotSpan span, ITextEdit edit, string blockStart, string blockEnd)
        {
            //sp. case no selection
            if (span.IsEmpty)
            {
                span = new SnapshotSpan(span.Start.GetContainingLine().Start + ScanToNonWhitespaceChar(span.Start.GetContainingLine()), span.End.GetContainingLine().End);
            }

            // add start comment
            edit.Insert(span.Start, blockStart);
            // add end comment
            edit.Insert(span.End, blockEnd);

            return span;
        }

        protected virtual SnapshotSpan UncommentSpan(SnapshotSpan span, ITextEdit edit)
        {
            span = span.TranslateTo(edit.Snapshot, SpanTrackingMode.EdgeExclusive);
            bool useLineComments = true;
            var startContainingLine = span.Start.GetContainingLine();
            var endContainingLine = span.End.GetContainingLine();

            // special case: empty span
            if (span.IsEmpty)
            {
                if (useLineComments)
                    span = UncommentLines(span, edit, CommentFormat.LineStart);
            }
            else
            {
                string textblock = span.GetText().Trim();

                if (!string.IsNullOrEmpty(CommentFormat.BlockStart)
                    && !string.IsNullOrEmpty(CommentFormat.BlockEnd)
                    && textblock.Length >= CommentFormat.BlockStart.Length + CommentFormat.BlockEnd.Length
                    && textblock.StartsWith(CommentFormat.BlockStart)
                    && textblock.EndsWith(CommentFormat.BlockEnd))
                {
                    TrimSpan(ref span);
                    span = UncommentBlock(span, edit, CommentFormat.BlockStart, CommentFormat.BlockEnd);
                }
                else if (useLineComments && !string.IsNullOrEmpty(CommentFormat.LineStart))
                {
                    span = UncommentLines(span, edit, CommentFormat.LineStart);
                }
            }

            return span;
        }

        protected virtual SnapshotSpan UncommentLines(SnapshotSpan span, ITextEdit edit, string lineComment)
        {
            if (span.End.GetContainingLine().LineNumber > span.Start.GetContainingLine().LineNumber && span.End == span.End.GetContainingLine().Start)
            {
                SnapshotPoint start = span.Start;
                SnapshotPoint end = span.Snapshot.GetLineFromLineNumber(span.End.GetContainingLine().LineNumber - 1).Start;
                if (end < start)
                    start = end;

                span = new SnapshotSpan(start, end);
            }

            // Remove line comments
            int clen = lineComment.Length;
            for (int line = span.Start.GetContainingLine().LineNumber; line <= span.End.GetContainingLine().LineNumber; line++)
            {
                int i = ScanToNonWhitespaceChar(span.Snapshot.GetLineFromLineNumber(line));
                string text = span.Snapshot.GetLineFromLineNumber(line).GetText();
                if ((text.Length > i + clen) && text.Substring(i, clen) == lineComment)
                {
                    // remove line comment.
                    edit.Delete(span.Snapshot.GetLineFromLineNumber(line).Start.Position + i, clen);
                }
            }

            span = new SnapshotSpan(span.Start.GetContainingLine().Start, span.End.GetContainingLine().End);
            return span;
        }

        protected virtual SnapshotSpan UncommentBlock(SnapshotSpan span, ITextEdit edit, string blockStart, string blockEnd)
        {
            int startLen = span.Start.GetContainingLine().Length;
            int endLen = span.End.GetContainingLine().Length;

            SnapshotSpan result = span;

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
                    }
                }
            }

            return result;
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

        protected static int ScanToNonWhitespaceChar(ITextSnapshotLine line)
        {
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
