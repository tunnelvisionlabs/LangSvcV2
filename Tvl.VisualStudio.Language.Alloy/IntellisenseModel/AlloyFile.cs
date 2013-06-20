namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

    internal sealed class AlloyFile : Element
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
                return this;
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
