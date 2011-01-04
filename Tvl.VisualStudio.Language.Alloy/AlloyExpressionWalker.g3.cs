namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Alloy.IntellisenseModel;
    using System.Collections.Generic;

    partial class AlloyExpressionWalker
    {
        private readonly SnapshotSpan _sourceSpan;

        public AlloyExpressionWalker(SnapshotSpan sourceSpan, ITreeNodeStream input)
            : this(input)
        {
            _sourceSpan = sourceSpan;
        }

        private SnapshotSpan? GetSpan(CommonTree tree)
        {
            if (tree == null)
                return null;

            return GetSpan(tree.Token);
        }

        private SnapshotSpan? GetSpan(IToken token)
        {
            if (token == null || _sourceSpan.Snapshot == null)
                return null;

            SnapshotPoint start = _sourceSpan.Start + token.StartIndex;
            SnapshotPoint end = _sourceSpan.Start + token.StopIndex + 1;
            return new SnapshotSpan(start, end);
        }

        private Expression MakeIntegerConstant(CommonTree tree)
        {
            int value = int.Parse(tree.Text);
            return Expression.Constant(value, GetSpan(tree));
        }

        private Expression MakeBoxJoinOrCall(Expression left, List<Expression> right, SnapshotSpan? openBracket, SnapshotSpan? closeBracket)
        {
            throw new NotImplementedException();
        }
    }
}
