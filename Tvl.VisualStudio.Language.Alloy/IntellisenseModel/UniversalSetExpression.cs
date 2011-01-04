namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public class UniversalSetExpression : NameExpression
    {
        public UniversalSetExpression(SnapshotSpan? span)
            : base(ExpressionType.UniversalSet, "univ", span)
        {
        }
    }
}
