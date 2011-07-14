namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public abstract class Expression
    {
        private readonly ExpressionType _nodeType;

        protected Expression(ExpressionType nodeType)
        {
            _nodeType = nodeType;
        }

        public virtual ExpressionType NodeType
        {
            get
            {
                return _nodeType;
            }
        }

        public virtual SnapshotSpan? Span
        {
            get
            {
                return null;
            }
        }

        public static ConstantExpression Constant(object value, SnapshotSpan? span)
        {
            return new ConstantExpression(value, span);
        }

        public static NameExpression Name(string name, SnapshotSpan? span)
        {
            return new NameExpression(name, span);
        }

        public static BlockExpression Block(IEnumerable<Expression> expressions, SnapshotSpan? openBraceSpan, SnapshotSpan? closeBraceSpan)
        {
            return new BlockExpression(expressions, openBraceSpan, closeBraceSpan);
        }

        public static BlockExpression Group(Expression expression, SnapshotSpan? openBraceSpan, SnapshotSpan? closeBraceSpan)
        {
            throw new NotImplementedException();
        }

        public static Expression Let(IEnumerable<Expression> declarations, SnapshotSpan? operatorSpan)
        {
            throw new NotImplementedException();
        }

        public static Expression Quant(ExpressionType nodeType, IEnumerable<Expression> declarations, SnapshotSpan? operatorSpan)
        {
            throw new NotImplementedException();
        }

        public static Expression EmptySet(SnapshotSpan? span)
        {
            return new EmptySetExpression(span);
        }

        public static Expression UniversalSet(SnapshotSpan? span)
        {
            return new UniversalSetExpression(span);
        }

        public static Expression IdentitySet(SnapshotSpan? span)
        {
            return new IdentitySetExpression(span);
        }

        public static DeclarationExpression Declaration(
            IEnumerable<Expression> names,
            Expression type,
            bool isPrivate,
            bool isDisjointNames,
            bool isDisjointType,
            SnapshotSpan? privateSpan,
            SnapshotSpan? disjointNamesSpan,
            SnapshotSpan? colonSpan,
            SnapshotSpan? disjointTypeSpan)
        {
            throw new NotImplementedException();
        }

        public static BinaryExpression Union(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.Union, left, right, operatorSpan);
        }

        public static BinaryExpression Intersection(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.Intersection, left, right, operatorSpan);
        }

        public static BinaryExpression Difference(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.Difference, left, right, operatorSpan);
        }

        public static BinaryExpression Subset(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.Subset, left, right, operatorSpan);
        }

        public static BinaryExpression Equal(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.Equal, left, right, operatorSpan);
        }

        public static RelationExpression ArrowProduct(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            throw new NotImplementedException();
        }

        public static CallExpression Call(Expression left, IEnumerable<Expression> right, SnapshotSpan? operatorSpan)
        {
            return new CallExpression(left, right, operatorSpan);
        }

        public static BinaryExpression DotJoin(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.DotJoin, left, right, operatorSpan);
        }

        public static BinaryExpression BoxJoin(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.BoxJoin, left, right, operatorSpan);
        }

        public static UnaryExpression Transpose(Expression expression, SnapshotSpan? operatorSpan)
        {
            return MakeUnary(ExpressionType.Transpose, expression, operatorSpan);
        }

        public static UnaryExpression TransitiveClosure(Expression expression, SnapshotSpan? operatorSpan)
        {
            return MakeUnary(ExpressionType.TransitiveClosure, expression, operatorSpan);
        }

        public static UnaryExpression ReflexiveTransitiveClosure(Expression expression, SnapshotSpan? operatorSpan)
        {
            return MakeUnary(ExpressionType.ReflexiveTransitiveClosure, expression, operatorSpan);
        }

        public static BinaryExpression DomainRestriction(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.DomainRestriction, left, right, operatorSpan);
        }

        public static BinaryExpression RangeRestriction(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.RangeRestriction, left, right, operatorSpan);
        }

        public static BinaryExpression Override(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.Override, left, right, operatorSpan);
        }

        public static BinaryExpression And(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.And, left, right, operatorSpan);
        }

        public static BinaryExpression AndAlso(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.AndAlso, left, right, operatorSpan);
        }

        public static BinaryExpression OrElse(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.OrElse, left, right, operatorSpan);
        }

        public static ConditionExpression Implies(Expression condition, Expression then, Expression @else, SnapshotSpan? impliesOperatorSpan, SnapshotSpan? elseOperatorSpan)
        {
            throw new NotImplementedException();
        }

        public static BinaryExpression Iff(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.Iff, left, right, operatorSpan);
        }

        public static UnaryExpression Count(Expression expression, SnapshotSpan? operatorSpan)
        {
            return MakeUnary(ExpressionType.Count, expression, operatorSpan);
        }

        public static UnaryExpression Not(Expression expression, SnapshotSpan? operatorSpan)
        {
            return MakeUnary(ExpressionType.Not, expression, operatorSpan);
        }

        public static BinaryExpression LeftShift(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.LeftShift, left, right, operatorSpan);
        }

        public static BinaryExpression RightShift(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.RightShift, left, right, operatorSpan);
        }

        public static BinaryExpression RightRotate(Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return MakeBinary(ExpressionType.RightRotate, left, right, operatorSpan);
        }

        public static BinaryExpression MakeBinary(ExpressionType type, Expression left, Expression right, SnapshotSpan? operatorSpan)
        {
            return new BinaryExpression(type, left, right, operatorSpan);
        }

        public static UnaryExpression MakeUnary(ExpressionType type, Expression expression, SnapshotSpan? operatorSpan)
        {
            return new UnaryExpression(type, expression, operatorSpan);
        }

        protected static SnapshotSpan? TryGetSpan(Expression expression)
        {
            if (expression == null)
                return null;

            return expression.Span;
        }

        protected static SnapshotSpan MergeSpans(IEnumerable<SnapshotSpan> expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");

            SnapshotSpan? mergedSpan = null;
            foreach (var span in expressions)
            {
                if (!mergedSpan.HasValue)
                {
                    mergedSpan = span;
                    continue;
                }

                if (span.Start < mergedSpan.Value.Start)
                    mergedSpan = new SnapshotSpan(span.Start, mergedSpan.Value.End);

                if (span.End > mergedSpan.Value.End)
                    mergedSpan = new SnapshotSpan(mergedSpan.Value.Start, span.End);
            }

            return mergedSpan.Value;
        }

        protected static SnapshotSpan? MergeSpans(IEnumerable<SnapshotSpan?> expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");

            SnapshotSpan[] spans = expressions.Where(i => i.HasValue).Select(i => i.Value).ToArray();
            if (spans.Length == 0)
                return null;

            return MergeSpans(spans.AsEnumerable());
        }

        protected static SnapshotSpan MergeSpans(params SnapshotSpan[] expressions)
        {
            return MergeSpans(expressions.AsEnumerable());
        }

        protected static SnapshotSpan? MergeSpans(params SnapshotSpan?[] expressions)
        {
            return MergeSpans(expressions.AsEnumerable());
        }
    }
}
