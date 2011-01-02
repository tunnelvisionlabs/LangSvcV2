namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class Module : Element
    {
        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override AlloyFile File
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsExternallyVisible
        {
            get
            {
                return true;
            }
        }
    }
}
