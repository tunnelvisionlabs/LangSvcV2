namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using Microsoft.VisualStudio.Text;

    public class UnaryExpression : Expression
    {
        private readonly Expression _expression;
        private readonly SnapshotSpan? _operatorSpan;

        public UnaryExpression(ExpressionType type, Expression expression, SnapshotSpan? operatorSpan)
            : base(type)
        {
            this._expression = expression;
            this._operatorSpan = operatorSpan;
        }

        public Expression Expression
        {
            get
            {
                return _expression;
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
                return MergeSpans(TryGetSpan(Expression), OperatorSpan);
            }
        }

        public override string ToString()
        {
            string operatorText = OperatorSpan.HasValue ? OperatorSpan.Value.GetText() : NodeType.ToString();
            return string.Format("({0} {1})", operatorText, Expression);
        }
    }
}
