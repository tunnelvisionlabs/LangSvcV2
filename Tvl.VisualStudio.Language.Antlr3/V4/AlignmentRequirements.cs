namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;

    [Flags]
    internal enum AlignmentRequirements
    {
        None = 0,

        UseAncestor = 0x0001,

        PriorSibling = 0x0002,

        IgnoreTree = 0x0004,
    }
}
