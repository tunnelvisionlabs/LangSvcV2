namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using Microsoft.VisualStudio.Text;

    public class UniversalSetExpression : NameExpression
    {
        public UniversalSetExpression(SnapshotSpan? span)
            : base(ExpressionType.UniversalSet, "univ", span)
        {
        }
    }
}
