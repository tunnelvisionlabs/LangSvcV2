namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

    [Flags, Serializable]
    internal enum DeclarationAttributes
    {
        None,
        Private = 0x0001,
    }
}
