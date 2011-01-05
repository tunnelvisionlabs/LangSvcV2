namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Alloy.IntellisenseModel;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;

    [Export]
    internal sealed partial class AlloyIntellisenseCache
    {
        private readonly ReaderWriterLockSlim _updateLock = new ReaderWriterLockSlim();

        private readonly Dictionary<AlloyFileReference, AlloyFile> _files = new Dictionary<AlloyFileReference, AlloyFile>(UniqueFileComparer.Default);

        [Import]
        public IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService
        {
            get;
            private set;
        }

        public SnapshotSpan? GetExpressionSpan(SnapshotPoint point)
        {
            var span = new SnapshotSpan(point, point);
            var buffer = span.Snapshot.TextBuffer;
            var aggregator = BufferTagAggregatorFactoryService.CreateTagAggregator<AlloyIntellisenseTag>(buffer);
            IMappingTagSpan<AlloyIntellisenseTag>[] tags = aggregator.GetTags(span).Where(i => i.Span.GetSpans(point.Snapshot.TextBuffer).Contains(span)).ToArray();
            IMappingTagSpan<AlloyIntellisenseTag>[] expressionTags = tags.Where(i => i.Tag.Type == AlloyIntellisenseTagType.Expression).ToArray();
            IMappingTagSpan<AlloyIntellisenseTag> tag = expressionTags.OrderBy(i => i.Span, MappingSpanComparer.Default).FirstOrDefault();
            if (tag == null)
                return null;

            NormalizedSnapshotSpanCollection spans = tag.Span.GetSpans(buffer);
            if (spans.Count == 0)
                return null;

            if (spans.Count > 1)
                throw new NotSupportedException();

            return spans[0];
        }

        public bool TryResolveContext(AlloyPositionReference position, out Element element)
        {
            element = null;
            return false;
        }

        public Expression ParseExpression(VirtualSnapshotSpan expressionSpan)
        {
            return ParseExpression(expressionSpan.SnapshotSpan);
        }

        public Expression ParseExpression(SnapshotSpan expressionSpan)
        {
            string text = expressionSpan.GetText();
            string sourceName = null;
            AlloyLexer lexer = new AlloyLexer(new ANTLRStringStream(text, sourceName));
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            AlloyParser parser = new AlloyParser(tokens);
            AlloyParser.expr_return tree = parser.expr();
            BufferedTreeNodeStream treeNodeStream = new BufferedTreeNodeStream(tree.Tree);
            AlloyExpressionWalker walker = new AlloyExpressionWalker(expressionSpan, treeNodeStream);
            Expression result = walker.expr();
            return result;
        }

        private class MappingSpanComparer : IComparer<IMappingSpan>
        {
            private static readonly MappingSpanComparer _default = new MappingSpanComparer();

            public static MappingSpanComparer Default
            {
                get
                {
                    return _default;
                }
            }

            public int Compare(IMappingSpan x, IMappingSpan y)
            {
                throw new NotImplementedException();
            }
        }
    }
}
