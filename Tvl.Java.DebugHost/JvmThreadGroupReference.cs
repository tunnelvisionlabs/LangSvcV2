namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugHost.Interop;

    public class JvmThreadGroupReference : JvmObjectReference
    {
        internal JvmThreadGroupReference(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, jthreadGroup handle, bool freeLocalReference)
            : base(environment, nativeEnvironment, handle, freeLocalReference)
        {
            Contract.Requires(environment != null);
            Contract.Requires(nativeEnvironment != null);
            Contract.Requires(handle != jthreadGroup.Null);
        }

        internal JvmThreadGroupReference(JvmEnvironment environment, SafeJvmWeakGlobalReferenceHandle handle)
            : base(environment, handle)
        {
            Contract.Requires(environment != null);
            Contract.Requires(handle != null);
        }

        public static JvmThreadGroupReference FromHandle(JvmEnvironment environment, JNIEnvHandle jniEnv, jthreadGroup objectHandle, bool freeLocalReference)
        {
            if (objectHandle == jobject.Null)
                return null;

            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            return new JvmThreadGroupReference(environment, nativeEnvironment, objectHandle, freeLocalReference);
        }

        public string GetName()
        {
            return GetInfo().Name;
        }

        public JvmThreadGroupReference GetParent()
        {
            return GetInfo().Parent;
        }

        public void Suspend()
        {
            Environment.SuspendThreads(GetThreads());
        }

        public void Resume()
        {
            Environment.ResumeThreads(GetThreads());
        }

        public ReadOnlyCollection<JvmThreadGroupReference> GetThreadGroups()
        {
            DisposableObjectCollection<JvmThreadReference> threads = null;
            DisposableObjectCollection<JvmThreadGroupReference> groups = new DisposableObjectCollection<JvmThreadGroupReference>();
            Environment.GetThreadGroupChildren(this, threads, groups);
            return new ReadOnlyCollection<JvmThreadGroupReference>(groups);
        }

        public ReadOnlyCollection<JvmThreadReference> GetThreads()
        {
            DisposableObjectCollection<JvmThreadReference> threads = new DisposableObjectCollection<JvmThreadReference>();
            DisposableObjectCollection<JvmThreadGroupReference> groups = null;
            Environment.GetThreadGroupChildren(this, threads, groups);
            return new ReadOnlyCollection<JvmThreadReference>(threads);
        }

        private JvmThreadGroupInfo GetInfo()
        {
            return Environment.GetThreadGroupInfo(this);
        }
    }
}
