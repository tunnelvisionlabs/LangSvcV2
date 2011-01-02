namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

    [Flags, Serializable]
    internal enum FactAttributes
    {
        None = 0,

        FactTypeMask = 0x0001,
        Fact = 0x0000,
        Assertion = 0x0001,

        Anonymous = 0x0002,
    }
}
