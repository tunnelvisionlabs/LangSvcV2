namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using Microsoft.VisualStudio.Text;

    public class IdentitySetExpression : NameExpression
    {
        public IdentitySetExpression(SnapshotSpan? span)
            : base(ExpressionType.IdentitySet, "iden", span)
        {
        }
    }
}
