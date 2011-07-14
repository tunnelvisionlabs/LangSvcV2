namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using System.Collections.ObjectModel;

    public class CallExpression : Expression
    {
        private readonly Expression _target;
        private readonly List<Expression> _arguments;
        private readonly SnapshotSpan? _operatorSpan;

        public CallExpression(Expression target, IEnumerable<Expression> arguments, SnapshotSpan? operatorSpan)
            : base(ExpressionType.Call)
        {
            _target = target;
            _arguments = arguments.ToList();
            _operatorSpan = operatorSpan;
        }

        public Expression Target
        {
            get
            {
                return _target;
            }
        }

        public ReadOnlyCollection<Expression> Arguments
        {
            get
            {
                return _arguments.AsReadOnly();
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
                return MergeSpans(TryGetSpan(_target), MergeSpans(_arguments.Select(TryGetSpan)), _operatorSpan);
            }
        }

        public override string ToString()
        {
            return string.Format("([] {0} {1})", Target, string.Join(" ", Arguments));
        }
    }
}
