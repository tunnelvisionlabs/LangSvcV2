namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Flags]
    public enum GlobalAllocFlags
    {
        None = 0,
        Fixed = 0x0000,
        Movable = 0x0002,
        ZeroInit = 0x0040,

        Handle = Movable | ZeroInit,
        Pointer = Fixed | ZeroInit,
    }
}
