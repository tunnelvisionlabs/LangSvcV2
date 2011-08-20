namespace Tvl.Java.DebugHost
{
    using Tvl.Java.DebugHost.Interop;
    using JvmClassRemoteHandle = Tvl.Java.DebugHost.Services.JvmClassRemoteHandle;

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

        public static implicit operator JvmClassRemoteHandle(JvmClassReference @class)
        {
            return new JvmClassRemoteHandle((jclass)@class);
        }

        public static JvmClassReference FromHandle(JvmEnvironment environment, JNIEnvHandle jniEnv, jclass classHandle)
        {
            if (classHandle == jclass.Null)
                return null;

            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            return new JvmClassReference(environment, nativeEnvironment, classHandle);
        }
    }
}
