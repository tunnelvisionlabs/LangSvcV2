namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class RelationExpression : Expression
    {
        public RelationExpression()
            : base(ExpressionType.Relation)
        {
        }
    }
}
