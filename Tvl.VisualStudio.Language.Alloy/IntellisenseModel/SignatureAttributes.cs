namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

    [Flags, Serializable]
    internal enum SignatureAttributes
    {
        None = 0,

        MultiplicityMask = 0x0003,
        ZeroOrMore = 0x0000,
        ZeroOrOne = 0x0001,
        One = 0x0002,
        OneOrMore = 0x0003,

        Private = 0x0004,
        Abstract = 0x0008,
        Enum = 0x0010,
    }
}
