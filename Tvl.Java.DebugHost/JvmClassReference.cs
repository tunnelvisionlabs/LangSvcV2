namespace Tvl.Java.DebugHost
{
    using System;
    using Tvl.Java.DebugHost.Interop;

    public class JvmClassReference : JvmObjectReference
    {
        internal JvmClassReference(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, jclass handle)
            : base(environment, nativeEnvironment, handle)
        {
        }

        internal JvmClassReference(JvmEnvironment environment, SafeJvmGlobalReferenceHandle handle)
            : base(environment, handle)
        {
        }
    }
}
