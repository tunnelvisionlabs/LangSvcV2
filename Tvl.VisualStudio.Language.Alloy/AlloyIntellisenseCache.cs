namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Alloy.IntellisenseModel;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;

    [Export]
    internal sealed partial class AlloyIntellisenseCache
    {
        private readonly ReaderWriterLockSlim _updateLock = new ReaderWriterLockSlim();

        private readonly Dictionary<AlloyFileReference, AlloyFile> _files = new Dictionary<AlloyFileReference, AlloyFile>(UniqueFileComparer.Default);

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
    }
}
