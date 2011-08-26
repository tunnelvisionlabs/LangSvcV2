namespace Tvl.Java.DebugHost
{
    using Tvl.Java.DebugHost.Interop;
    using JvmClassRemoteHandle = Tvl.Java.DebugHost.Services.JvmClassRemoteHandle;

    public class JvmClassReference : JvmObjectReference
    {
        internal JvmClassReference(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, jclass handle, bool freeLocalReference)
            : base(environment, nativeEnvironment, handle, freeLocalReference)
        {
        }

        internal JvmClassReference(JvmEnvironment environment, SafeJvmWeakGlobalReferenceHandle handle)
            : base(environment, handle)
        {
        }

        public static implicit operator JvmClassRemoteHandle(JvmClassReference @class)
        {
            return new JvmClassRemoteHandle((jclass)@class);
        }

        public static explicit operator jclass(JvmClassReference @class)
        {
            if (@class == null)
                return jclass.Null;

            return new jclass(@class.Handle.DangerousGetHandle());
        }

        public static JvmClassReference FromHandle(JvmEnvironment environment, JNIEnvHandle jniEnv, jclass classHandle, bool freeLocalReference)
        {
            if (classHandle == jclass.Null)
                return null;

            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            return new JvmClassReference(environment, nativeEnvironment, classHandle, freeLocalReference);
        }
    }
}
