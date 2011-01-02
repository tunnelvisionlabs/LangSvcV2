namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class Fact : FactBase
    {
        public Fact(string name, AlloyFile file)
            : base(name, file)
        {
        }

        public override FactAttributes Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
