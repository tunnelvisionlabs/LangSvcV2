namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;

    internal class Enum : Signature
    {
        public Enum(string name, AlloyFile file)
            : base(name, file, SignatureAttributes.Abstract | SignatureAttributes.Enum)
        {
        }

        public IEnumerable<IElementReference<Signature>> Members
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
