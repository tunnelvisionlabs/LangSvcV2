namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugHost.Interop;

    public class JvmThreadReference : JvmObjectReference
    {
        internal JvmThreadReference(JvmEnvironment environment, JvmNativeEnvironment nativeEnvironment, jthread handle)
            : base(environment, nativeEnvironment, handle)
        {
            Contract.Requires(environment != null);
            Contract.Requires(nativeEnvironment != null);
            Contract.Requires(handle != jthread.Null);
        }

        internal JvmThreadReference(JvmEnvironment environment, SafeJvmGlobalReferenceHandle handle)
            : base(environment, handle)
        {
            Contract.Requires(environment != null);
            Contract.Requires(handle != null);
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
            jvmtiThreadInfo info = Environment.GetThreadInfo(this);
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
            jvmtiThreadInfo threadInfo = Environment.GetThreadInfo(this);
            return new JvmThreadGroupReference(Environment, Handle.NativeEnvironment, threadInfo._threadGroup);
        }
    }
}
