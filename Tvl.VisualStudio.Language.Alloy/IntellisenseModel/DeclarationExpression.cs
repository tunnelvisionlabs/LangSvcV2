namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DeclarationExpression : Expression
    {
        public DeclarationExpression()
            : base(ExpressionType.Declaration)
        {
        }
    }
}
