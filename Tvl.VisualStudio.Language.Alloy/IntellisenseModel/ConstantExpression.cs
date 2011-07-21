namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using Microsoft.VisualStudio.Text;

    public class ConstantExpression : Expression
    {
        private readonly object _value;
        private readonly SnapshotSpan? _span;

        public ConstantExpression(object value, SnapshotSpan? span)
            : base(ExpressionType.Constant)
        {
            _value = value;
            _span = span;
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        public override SnapshotSpan? Span
        {
            get
            {
                return _span;
            }
        }

        public override string ToString()
        {
            if (Value == null)
                return "<null>";

            return _value.ToString();
        }
    }
}
