namespace Tvl.Java.DebugHost.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public struct JavaVMHandle
    {
        public IntPtr Handle;

        public JavaVMHandle(IntPtr handle)
        {
            Handle = handle;
        }
    }
}
