namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public class IdentitySetExpression : NameExpression
    {
        public IdentitySetExpression(SnapshotSpan? span)
            : base(ExpressionType.IdentitySet, "iden", span)
        {
        }
    }
}
