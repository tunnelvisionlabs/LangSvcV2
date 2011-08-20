namespace Tvl.Java.DebugHost
{
    using System;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugHost.Interop;

    public class JvmNativeEnvironment
    {
        private readonly JvmEnvironment _environment;
        private readonly JNIEnvHandle _nativeEnvironmentHandle;
        private readonly jniNativeInterface _nativeInterface;

        internal JvmNativeEnvironment(JvmEnvironment environment, JNIEnvHandle nativeEnvironmentHandle, jniNativeInterface nativeInterface)
        {
            Contract.Requires<ArgumentNullException>(environment != null, "environment");
            Contract.Requires<ArgumentException>(nativeEnvironmentHandle != JNIEnvHandle.Null);

            _environment = environment;
            _nativeEnvironmentHandle = nativeEnvironmentHandle;
            _nativeInterface = nativeInterface;
        }

        internal JNIEnvHandle Handle
        {
            get
            {
                return _nativeEnvironmentHandle;
            }
        }

        internal SafeJvmGlobalReferenceHandle NewGlobalReference(jobject @object)
        {
            return new SafeJvmGlobalReferenceHandle(this, _nativeInterface.NewGlobalRef(_nativeEnvironmentHandle, @object), true);
        }

        internal void DeleteGlobalReference(jobject reference)
        {
            if (!AgentExports.IsLoaded)
                return;

            _nativeInterface.DeleteGlobalRef(_nativeEnvironmentHandle, reference);
        }
    }
}
