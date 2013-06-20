namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using Microsoft.VisualStudio.Text;

    public class BinaryExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly SnapshotSpan? _operatorSpan;

        public BinaryExpression(ExpressionType type, Expression left, Expression right, SnapshotSpan? operatorSpan)
            : base(type)
        {
            this._left = left;
            this._right = right;
            this._operatorSpan = operatorSpan;
        }

        public Expression Left
        {
            get
            {
                return _left;
            }
        }

        public Expression Right
        {
            get
            {
                return _right;
            }
        }

        public SnapshotSpan? OperatorSpan
        {
            get
            {
                return _operatorSpan;
            }
        }

        public override SnapshotSpan? Span
        {
            get
            {
                return MergeSpans(TryGetSpan(Left), TryGetSpan(Right), OperatorSpan);
            }
        }

        public override string ToString()
        {
            string operatorText = OperatorSpan.HasValue ? OperatorSpan.Value.GetText() : NodeType.ToString();
            return string.Format("({0} {1} {2})", operatorText, Left, Right);
        }
    }
}
