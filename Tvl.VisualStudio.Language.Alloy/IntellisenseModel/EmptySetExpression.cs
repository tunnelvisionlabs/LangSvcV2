namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using Microsoft.VisualStudio.Text;

    public class EmptySetExpression : NameExpression
    {
        public EmptySetExpression(SnapshotSpan? span)
            : base(ExpressionType.EmptySet, "none", span)
        {
        }
    }
}
