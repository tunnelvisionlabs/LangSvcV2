namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public class ConditionExpression : Expression
    {
        private readonly Expression _condition;
        private readonly Expression _then;
        private readonly Expression _else;
        private readonly SnapshotSpan? _impliesOperatorSpan;
        private readonly SnapshotSpan? _elseOperatorSpan;

        public ConditionExpression(Expression condition, Expression then, Expression @else, SnapshotSpan? impliesOperatorSpan, SnapshotSpan? elseOperatorSpan)
            : base(ExpressionType.Implies)
        {
            _condition = condition;
            _then = then;
            _else = @else;
            _impliesOperatorSpan = impliesOperatorSpan;
            _elseOperatorSpan = elseOperatorSpan;
        }

        public Expression Condition
        {
            get
            {
                return _condition;
            }
        }

        public Expression Then
        {
            get
            {
                return _then;
            }
        }

        public Expression Else
        {
            get
            {
                return _else;
            }
        }

        public SnapshotSpan? ImpliesOperatorSpan
        {
            get
            {
                return _impliesOperatorSpan;
            }
        }

        public SnapshotSpan? ElseOperatorSpan
        {
            get
            {
                return _elseOperatorSpan;
            }
        }

        public override SnapshotSpan? Span
        {
            get
            {
                return MergeSpans(TryGetSpan(Condition), TryGetSpan(Then), TryGetSpan(Else), ImpliesOperatorSpan, ElseOperatorSpan);
            }
        }

        public override string ToString()
        {
            string impliesOperatorText = ImpliesOperatorSpan.HasValue ? ImpliesOperatorSpan.Value.GetText() : NodeType.ToString();
            string elseOperatorText = ElseOperatorSpan.HasValue ? ElseOperatorSpan.Value.GetText() : "else";
            bool hasElse = Else != null || ElseOperatorSpan != null;
            string elseText = hasElse ? string.Format(" ({0} {1})", elseOperatorText, Else) : string.Empty;
            return string.Format("({0} {1} {2}{3})", impliesOperatorText, Condition, Then, elseText);
        }
    }
}
