namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

    internal class Assertion : FactBase
    {
        public Assertion(string name, AlloyFile file)
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
