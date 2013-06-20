namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

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
