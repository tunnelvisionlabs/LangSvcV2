namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class Function : FunctionBase
    {
        public Function(string name, AlloyFile file)
            : base(name, file)
        {
        }

        public override FunctionAttributes Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
