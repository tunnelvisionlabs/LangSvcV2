namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using Microsoft.VisualStudio.Text;

    public class NameExpression : Expression
    {
        private readonly string _name;
        private readonly SnapshotSpan? _span;

        public NameExpression(string name, SnapshotSpan? span)
            : this(ExpressionType.Name, name, span)
        {
        }

        protected NameExpression(ExpressionType nodeType, string name, SnapshotSpan? span)
            : base(nodeType)
        {
            _name = name;
            _span = span;
        }

        public new string Name
        {
            get
            {
                return _name;
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
            return Name;
        }
    }
}
