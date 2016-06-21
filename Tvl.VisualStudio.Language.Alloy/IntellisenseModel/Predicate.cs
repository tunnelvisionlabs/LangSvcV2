namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

    internal class Predicate : FunctionBase
    {
        public Predicate(string name, AlloyFile file)
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
