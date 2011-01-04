namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public class EmptySetExpression : NameExpression
    {
        public EmptySetExpression(SnapshotSpan? span)
            : base(ExpressionType.EmptySet, "none", span)
        {
        }
    }
}
