namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

    [Flags, Serializable]
    internal enum FunctionAttributes
    {
        None = 0,

        FunctionTypeMask = 0x0001,
        Function = 0x0000,
        Predicate = 0x0001,

        Private = 0x0002,
    }
}
