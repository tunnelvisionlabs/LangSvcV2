namespace Tvl.VisualStudio.Language.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;

    [Obsolete]
    public abstract class AntlrClassifierBase : IClassifier
    {
        private ITextSnapshot _multilineTokenReference;
        private List<SnapshotSpan> _multilineTokens = new List<SnapshotSpan>();

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            Contract.Ensures(Contract.Result<IList<ClassificationSpan>>() != null);

            UpdateMultilineTokens(ref span);

            ICharStream input = CreateInputStream(span);
            ITokenSource lexer = CreateLexer(input);
            List<ClassificationSpan> classificationSpans = new List<ClassificationSpan>();
            while (true)
            {
                var token = lexer.NextToken();
                if (token.Type == CharStreamConstants.EndOfFile)
                    break;

                if (token.StartIndex > span.End.Position)
                    break;

                var tokenClassificationSpans = GetClassificationSpansForToken(token, span.Snapshot);
                if (tokenClassificationSpans != null)
                    classificationSpans.AddRange(tokenClassificationSpans);

                if (token.StopIndex >= span.End.Position)
                    break;
            }

            if (classificationSpans.Count > 0)
            {
                int startPosition = classificationSpans[0].Span.Start.Position;
                int endPosition = classificationSpans[classificationSpans.Count - 1].Span.End.Position;
                int length = endPosition - startPosition;
                var finalSpan = new Span(startPosition, length);
                _multilineTokens.RemoveAll(classificationSpan => classificationSpan.IsEmpty || classificationSpan.IntersectsWith(finalSpan));
                _multilineTokens.AddRange(classificationSpans.Where(IsMultilineClassificationSpan).Select(classificationSpan => classificationSpan.Span));
            }

            return classificationSpans;
        }

        protected abstract ITokenSource CreateLexer(ICharStream input);

        protected virtual ICharStream CreateInputStream(SnapshotSpan span)
        {
            SnapshotCharStream input = new SnapshotCharStream(span.Snapshot);
            input.Seek(span.Start.Position);
            return input;
        }

        protected virtual IEnumerable<ClassificationSpan> GetClassificationSpansForToken(IToken token, ITextSnapshot snapshot)
        {
            Contract.Requires<ArgumentNullException>(token != null, "token");
            Contract.Requires<ArgumentNullException>(snapshot != null, "snapshot");
            Contract.Ensures(Contract.Result<IEnumerable<ClassificationSpan>>() != null);

            var classification = ClassifyToken(token);
            if (classification != null)
            {
                var span = new SnapshotSpan(snapshot, token.StartIndex, token.StopIndex - token.StartIndex + 1);
                return new ClassificationSpan[] { new ClassificationSpan(span, classification) };
            }

            return Enumerable.Empty<ClassificationSpan>();
        }

        protected virtual IClassificationType ClassifyToken(IToken token)
        {
            Contract.Requires<ArgumentNullException>(token != null, "token");

            return null;
        }

        protected virtual void OnClassificationChanged(ClassificationChangedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(e != null, "e");

            var t = ClassificationChanged;
            if (t != null)
                t(this, e);
        }

        private static bool IsMultilineClassificationSpan(ClassificationSpan span)
        {
            Contract.Requires<ArgumentNullException>(span != null, "span");

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
