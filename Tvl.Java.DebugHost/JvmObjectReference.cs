namespace Tvl.Java.DebugHost
{
    using System;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugHost.Interop;

    public class JvmObjectReference : IDisposable
    {
        private readonly JvmEnvironment _environment;
        private readonly SafeJvmGlobalReferenceHandle _handle;

        internal JvmObjectReference(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, jobject handle)
        {
            Contract.Requires<ArgumentNullException>(environment != null, "environment");
            Contract.Requires<ArgumentNullException>(nativeEnvironment != null, "nativeEnvironment");
            Contract.Requires<ArgumentException>(handle != jobject.Null);

            _environment = environment;
            _handle = nativeEnvironment.NewGlobalReference(handle);
        }

        internal JvmObjectReference(JvmEnvironment environment, SafeJvmGlobalReferenceHandle handle)
        {
            Contract.Requires<ArgumentNullException>(environment != null, "environment");
            Contract.Requires<ArgumentNullException>(handle != null, "handle");

            _environment = environment;
            _handle = handle;
        }

        public JvmEnvironment Environment
        {
            get
            {
                return _environment;
            }
        }

        protected internal SafeJvmGlobalReferenceHandle Handle
        {
            get
            {
                return _handle;
            }
        }

        public static JvmObjectReference FromHandle(JvmEnvironment environment, JNIEnvHandle jniEnv, jobject objectHandle)
        {
            if (objectHandle == jobject.Null)
                return null;

            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            return new JvmObjectReference(environment, nativeEnvironment, objectHandle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            SafeJvmGlobalReferenceHandle handle = _handle;
            if (handle != null)
                handle.Dispose();
        }
    }
}
