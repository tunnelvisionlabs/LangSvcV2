namespace Tvl.Java.DebugHost
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.Win32.SafeHandles;
    using Tvl.Java.DebugHost.Interop;

    public class SafeJvmWeakGlobalReferenceHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly JvmNativeEnvironment _nativeEnvironment;

        internal SafeJvmWeakGlobalReferenceHandle(JvmNativeEnvironment nativeEnvironment, jweak handle, bool ownsHandle)
            : base(ownsHandle)
        {
            Contract.Requires<ArgumentNullException>(nativeEnvironment != null, "nativeEnvironment");

            _nativeEnvironment = nativeEnvironment;
            SetHandle(handle.Handle);
        }

        protected override bool ReleaseHandle()
        {
            _nativeEnvironment.DeleteWeakGlobalReference(new jweak(handle));
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
