namespace Microsoft.VisualStudio.Project
{
    using System;
    using Microsoft.Win32.SafeHandles;

    public sealed class SafeGlobalAllocHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeGlobalAllocHandle(IntPtr handle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            UnsafeNativeMethods.GlobalFree(handle);
            return true;
        }
    }
}
