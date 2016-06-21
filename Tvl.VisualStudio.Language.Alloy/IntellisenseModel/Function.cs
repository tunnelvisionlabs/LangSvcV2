namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

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
