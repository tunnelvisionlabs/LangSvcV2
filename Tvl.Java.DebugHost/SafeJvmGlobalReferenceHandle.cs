namespace Tvl.Java.DebugHost
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.Win32.SafeHandles;
    using Tvl.Java.DebugHost.Interop;

    public class SafeJvmGlobalReferenceHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly JvmNativeEnvironment _nativeEnvironment;

        internal SafeJvmGlobalReferenceHandle(JvmNativeEnvironment nativeEnvironment, jobject handle, bool ownsHandle)
            : base(ownsHandle)
        {
            Contract.Requires<ArgumentNullException>(nativeEnvironment != null, "nativeEnvironment");

            _nativeEnvironment = nativeEnvironment;
            SetHandle(handle.Handle);
        }

        protected override bool ReleaseHandle()
        {
            _nativeEnvironment.DeleteGlobalReference(new jobject(handle));
            return true;
        }

        public JvmNativeEnvironment NativeEnvironment
        {
            get
            {
                return _nativeEnvironment;
            }
        }
    }
}
