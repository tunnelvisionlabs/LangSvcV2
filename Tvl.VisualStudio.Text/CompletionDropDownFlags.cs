namespace Tvl.VisualStudio.Text
{
    using System;

    [Flags]
    public enum CompletionDropDownFlags
    {
        None = 0,
        Enum = 0x0001,
        CompleteFullGenericName = 0x0002,
        AliasBuilder = 0x1000,
    }
}
