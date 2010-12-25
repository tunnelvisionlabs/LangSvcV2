namespace Tvl.VisualStudio.Text
{
    using System;

    [Flags]
    public enum CompletionFlags
    {
        None = 0,
        HasParameterInfo = 0x0001,
        TipIsQuickInfo = 0x0002,
        TipIsArrayTip = 0x0004,
        TipIsSyntaxTip = 0x0008,
    }
}
