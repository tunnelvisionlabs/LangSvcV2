namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.VisualStudio.Text;

    public class BlockExpression : Expression
    {
        private readonly ReadOnlyCollection<Expression> _expressions;
        private readonly SnapshotSpan? _openBraceSpan;
        private readonly SnapshotSpan? _closeBraceSpan;

        public BlockExpression(IEnumerable<Expression> expressions, SnapshotSpan? openBraceSpan, SnapshotSpan? closeBraceSpan)
            : base(ExpressionType.Block)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");

            _expressions = new ReadOnlyCollection<Expression>(expressions.ToArray());
            _openBraceSpan = openBraceSpan;
            _closeBraceSpan = closeBraceSpan;
        }

        public ReadOnlyCollection<Expression> Expressions
        {
            get
            {
                return _expressions;
            }
        }

        public SnapshotSpan? OpenBraceSpan
        {
            get
            {
                return _openBraceSpan;
            }
        }

        public SnapshotSpan? CloseBraceSpan
        {
            get
            {
                return _closeBraceSpan;
            }
        }

        public override SnapshotSpan? Span
        {
            get
            {
                SnapshotSpan? expressionsSpan = MergeSpans(Expressions.Select(i => i.Span));
                SnapshotSpan? blockSpan = MergeSpans(expressionsSpan, OpenBraceSpan, CloseBraceSpan);
                return blockSpan;
            }
        }
    }
}
