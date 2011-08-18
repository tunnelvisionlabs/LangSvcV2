namespace Tvl.Java.DebugHost
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.Win32.SafeHandles;

    public class SafeJvmAllocHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly JvmEnvironment _environment;

        public SafeJvmAllocHandle(JvmEnvironment environment, IntPtr handle, bool ownsHandle)
            : base(ownsHandle)
        {
            Contract.Requires<ArgumentNullException>(environment != null, "environment");

            _environment = environment;
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            _environment.Deallocate(handle);
            return true;
        }
    }
}
