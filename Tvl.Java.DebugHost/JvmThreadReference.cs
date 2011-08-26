namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugHost.Interop;
    using JvmThreadRemoteHandle = Tvl.Java.DebugHost.Services.JvmThreadRemoteHandle;
    using System.Collections.Generic;

    public class JvmThreadReference : JvmObjectReference
    {
        private static readonly List<SafeJvmWeakGlobalReferenceHandle> pinnedReferences = new List<SafeJvmWeakGlobalReferenceHandle>();

        internal JvmThreadReference(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, jthread handle, bool freeLocalReference)
            : base(environment, nativeEnvironment, handle, freeLocalReference)
        {
            Contract.Requires(environment != null);
            Contract.Requires(nativeEnvironment != null);
            Contract.Requires(handle != jthread.Null);

            // quick experiment to see if the lifetime of thread references is killing the process.
            pinnedReferences.Add(base.Handle);
        }

        internal JvmThreadReference(JvmEnvironment environment, SafeJvmWeakGlobalReferenceHandle handle)
            : base(environment, handle)
        {
            Contract.Requires(environment != null);
            Contract.Requires(handle != null);
        }

        public static implicit operator JvmThreadRemoteHandle(JvmThreadReference thread)
        {
            return new JvmThreadRemoteHandle((jthread)thread);
        }

        public static explicit operator jthread(JvmThreadReference thread)
        {
            if (thread == null)
                return jthread.Null;

            return new jthread(thread.Handle.DangerousGetHandle());
        }

        public static JvmThreadReference FromHandle(JvmEnvironment environment, JNIEnvHandle jniEnv, jthread threadHandle, bool freeLocalReference)
        {
            if (threadHandle == jthread.Null)
                return null;

            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            return new JvmThreadReference(environment, nativeEnvironment, threadHandle, freeLocalReference);
        }

        public ReadOnlyCollection<JvmStackFrame> GetFrames()
        {
            throw new NotImplementedException();
        }

        public bool GetIsAtBreakpoint()
        {
            throw new NotImplementedException();
        }

        public bool GetIsSuspended()
        {
            jvmtiThreadState state = Environment.GetThreadState(this);
            return (state & jvmtiThreadState.Suspended) != 0;
        }

        public string GetName()
        {
            jvmtiThreadInfo info;
            Environment.RawInterface.GetThreadInfo(Environment.Handle, (jthread)this, out info);
            return info.Name;
        }

        public DisposableObjectCollection<JvmMonitorInfo> GetOwnedMonitors()
        {
            return Environment.GetOwnedMonitorStackDepthInfo(this);
        }

        public void PopFrames(JvmStackFrame frame)
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            Environment.ResumeThread(this);
        }

        public JvmThreadStatus GetStatus()
        {
            throw new NotImplementedException();
        }

        public void Stop(JvmObjectReference throwable)
        {
            Environment.StopThread(this, throwable);
        }

        public void Suspend()
        {
            Environment.SuspendThread(this);
        }

        public int GetSuspendCount()
        {
            return Environment.GetSuspendCount(this);
        }

        public JvmThreadGroupReference GetThreadGroup()
        {
            throw new NotImplementedException();
            //JvmThreadInfo threadInfo = Environment.GetThreadInfo(this);
            //return threadInfo.ThreadGroup;
        }
    }
}
