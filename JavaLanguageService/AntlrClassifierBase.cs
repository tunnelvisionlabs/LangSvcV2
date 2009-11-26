namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;

    internal abstract class AntlrClassifierBase : IClassifier
    {
        private ITextSnapshot _multilineTokenReference;
        private List<SnapshotSpan> _multilineTokens = new List<SnapshotSpan>();

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            UpdateMultilineTokens(ref span);

            SnapshotCharStream input = new SnapshotCharStream(span.Snapshot);
            input.Seek(span.Start.Position);
            Lexer lexer = CreateLexer(input);
            List<ClassificationSpan> classificationSpans = new List<ClassificationSpan>();
            while (true)
            {
                var token = lexer.NextToken();
                if (token.Type == CharStreamConstants.EndOfFile)
                    break;

                if (token.StartIndex > span.End.Position)
                    break;

                var classificationSpan = GetClassificationSpanForToken(token, span.Snapshot);
                if (classificationSpan != null)
                    classificationSpans.Add(classificationSpan);

                if (token.StopIndex >= span.End.Position)
                    break;
            }

            if (classificationSpans.Count > 0)
            {
                var finalSpan = new Span(span.Start.Position, classificationSpans[classificationSpans.Count - 1].Span.End.Position - span.Start.Position);
                _multilineTokens.RemoveAll(classificationSpan => classificationSpan.IsEmpty || classificationSpan.IntersectsWith(finalSpan));
                _multilineTokens.AddRange(classificationSpans.Where(IsMultilineClassificationSpan).Select(classificationSpan => classificationSpan.Span));
            }

            return classificationSpans;
        }

        protected abstract Lexer CreateLexer(ICharStream input);

        protected virtual ClassificationSpan GetClassificationSpanForToken(IToken token, ITextSnapshot snapshot)
        {
            var classification = ClassifyToken(token);
            if (classification != null)
            {
                var span = new SnapshotSpan(snapshot, token.StartIndex, token.StopIndex - token.StartIndex + 1);
                return new ClassificationSpan(span, classification);
            }

            return null;
        }

        protected virtual IClassificationType ClassifyToken(IToken token)
        {
            return null;
        }

        protected virtual void OnClassificationChanged(ClassificationChangedEventArgs e)
        {
            var t = ClassificationChanged;
            if (t != null)
                t(this, e);
        }

        private static bool IsMultilineClassificationSpan(ClassificationSpan span)
        {
            if (span.Span.IsEmpty)
                return false;

            return span.Span.Start.GetContainingLine().LineNumber != span.Span.End.GetContainingLine().LineNumber;
        }

        private void UpdateMultilineTokens(ref SnapshotSpan spanToRefresh)
        {
            if (_multilineTokenReference == null)
            {
                spanToRefresh = new SnapshotSpan(spanToRefresh.Snapshot, 0, spanToRefresh.Snapshot.Length);
            }
            else
            {
                var line = spanToRefresh.Snapshot.GetLineFromPosition(spanToRefresh.Span.Start);
                if (line.Start != spanToRefresh.Start)
                    spanToRefresh = new SnapshotSpan(spanToRefresh.Snapshot, new Span(line.Start, spanToRefresh.Length + spanToRefresh.Start - line.Start));
            }

            for (int i = 0; i < _multilineTokens.Count; i++)
            {
                if (_multilineTokenReference != spanToRefresh.Snapshot)
                    _multilineTokens[i] = _multilineTokens[i].TranslateTo(spanToRefresh.Snapshot, SpanTrackingMode.EdgeExclusive);

                var span = _multilineTokens[i];
                if (span.OverlapsWith(spanToRefresh))
                    spanToRefresh = new SnapshotSpan(spanToRefresh.Snapshot, Math.Min(spanToRefresh.Start.Position, span.Start.Position), Math.Max(spanToRefresh.End.Position, span.End.Position) - Math.Min(spanToRefresh.Start.Position, span.Start.Position));
            }

            _multilineTokenReference = spanToRefresh.Snapshot;
        }
    }
}
