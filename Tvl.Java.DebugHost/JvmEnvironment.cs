namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugHost.Interop;
    using System.Runtime.InteropServices;
    using System.Collections.Concurrent;

    public class JvmEnvironment
    {
        private static readonly ConcurrentDictionary<jvmtiEnvHandle, JvmEnvironment> _environments =
            new ConcurrentDictionary<jvmtiEnvHandle, JvmEnvironment>();

        private readonly jvmtiEnvHandle _env;
        private readonly jvmtiInterface _rawInterface;
        private readonly JvmEventManager _eventManager;

        private JvmEnvironment(jvmtiEnvHandle env)
        {
            Contract.Requires<ArgumentException>(env != jvmtiEnvHandle.Null);

            jvmtiInterface rawInterface = (jvmtiInterface)Marshal.PtrToStructure(Marshal.ReadIntPtr(env.Handle), typeof(jvmtiInterface));

            Contract.Assert(rawInterface.GetCapabilities != null);
            Contract.Assert(rawInterface.Allocate != null);
            Contract.Assert(rawInterface.Deallocate != null);
            Contract.Assert(rawInterface.GetThreadState != null);
            Contract.Assert(rawInterface.GetCurrentThread != null);
            Contract.Assert(rawInterface.GetAllThreads != null);
            Contract.Assert(rawInterface.GetThreadInfo != null);
            Contract.Assert(rawInterface.RunAgentThread != null);
            Contract.Assert(rawInterface.SetThreadLocalStorage != null);
            Contract.Assert(rawInterface.GetThreadLocalStorage != null);
            Contract.Assert(rawInterface.GetTopThreadGroups != null);
            Contract.Assert(rawInterface.GetThreadGroupInfo != null);
            Contract.Assert(rawInterface.GetThreadGroupChildren != null);
            Contract.Assert(rawInterface.GetStackTrace != null);
            Contract.Assert(rawInterface.GetAllStackTraces != null);
            Contract.Assert(rawInterface.GetThreadListStackTraces != null);
            Contract.Assert(rawInterface.GetFrameCount != null);
            Contract.Assert(rawInterface.GetFrameLocation != null);
            Contract.Assert(rawInterface.ForceGarbageCollection != null);
            Contract.Assert(rawInterface.GetLoadedClasses != null);
            Contract.Assert(rawInterface.GetClassLoaderClasses != null);
            Contract.Assert(rawInterface.GetClassSignature != null);
            Contract.Assert(rawInterface.GetClassStatus != null);
            Contract.Assert(rawInterface.GetClassModifiers != null);
            Contract.Assert(rawInterface.GetClassMethods != null);
            Contract.Assert(rawInterface.GetClassFields != null);
            Contract.Assert(rawInterface.GetImplementedInterfaces != null);
            Contract.Assert(rawInterface.GetClassVersionNumbers != null);
            Contract.Assert(rawInterface.IsInterface != null);
            Contract.Assert(rawInterface.IsArrayClass != null);
            Contract.Assert(rawInterface.IsModifiableClass != null);
            Contract.Assert(rawInterface.GetClassLoader != null);
            Contract.Assert(rawInterface.GetObjectSize != null);
            Contract.Assert(rawInterface.GetObjectHashCode != null);
            Contract.Assert(rawInterface.GetFieldName != null);
            Contract.Assert(rawInterface.GetFieldDeclaringClass != null);
            Contract.Assert(rawInterface.GetFieldModifiers != null);
            Contract.Assert(rawInterface.GetMethodName != null);
            Contract.Assert(rawInterface.GetMethodDeclaringClass != null);
            Contract.Assert(rawInterface.GetMethodModifiers != null);
            Contract.Assert(rawInterface.GetMaxLocals != null);
            Contract.Assert(rawInterface.GetArgumentsSize != null);
            Contract.Assert(rawInterface.GetMethodLocation != null);
            Contract.Assert(rawInterface.IsMethodNative != null);
            Contract.Assert(rawInterface.IsMethodObsolete != null);
            Contract.Assert(rawInterface.CreateRawMonitor != null);
            Contract.Assert(rawInterface.DestroyRawMonitor != null);
            Contract.Assert(rawInterface.RawMonitorEnter != null);
            Contract.Assert(rawInterface.RawMonitorExit != null);
            Contract.Assert(rawInterface.RawMonitorWait != null);
            Contract.Assert(rawInterface.RawMonitorNotify != null);
            Contract.Assert(rawInterface.RawMonitorNotifyAll != null);
            Contract.Assert(rawInterface.SetJNIFunctionTable != null);
            Contract.Assert(rawInterface.GetJNIFunctionTable != null);
            Contract.Assert(rawInterface.SetEventCallbacks != null);
            Contract.Assert(rawInterface.SetEventNotificationMode != null);
            Contract.Assert(rawInterface.GenerateEvents != null);
            Contract.Assert(rawInterface.GetExtensionFunctions != null);
            Contract.Assert(rawInterface.GetExtensionEvents != null);
            Contract.Assert(rawInterface.SetExtensionEventCallback != null);
            Contract.Assert(rawInterface.GetPotentialCapabilities != null);
            Contract.Assert(rawInterface.AddCapabilities != null);
            Contract.Assert(rawInterface.RelinquishCapabilities != null);
            Contract.Assert(rawInterface.GetCapabilities != null);
            Contract.Assert(rawInterface.GetTimerInfo != null);
            Contract.Assert(rawInterface.GetTime != null);
            Contract.Assert(rawInterface.GetAvailableProcessors != null);
            Contract.Assert(rawInterface.AddToBootstrapClassLoaderSearch != null);
            Contract.Assert(rawInterface.AddToSystemClassLoaderSearch != null);
            Contract.Assert(rawInterface.GetSystemProperties != null);
            Contract.Assert(rawInterface.GetSystemProperty != null);
            Contract.Assert(rawInterface.SetSystemProperty != null);
            Contract.Assert(rawInterface.GetPhase != null);
            Contract.Assert(rawInterface.DisposeEnvironment != null);
            Contract.Assert(rawInterface.SetEnvironmentLocalStorage != null);
            Contract.Assert(rawInterface.GetEnvironmentLocalStorage != null);
            Contract.Assert(rawInterface.GetVersionNumber != null);
            Contract.Assert(rawInterface.GetErrorName != null);
            Contract.Assert(rawInterface.SetVerboseFlag != null);
            Contract.Assert(rawInterface.GetJLocationFormat != null);

            jvmtiCapabilities capabilities;
            if (rawInterface.GetPotentialCapabilities(env, out capabilities) != jvmtiError.None)
                throw new InvalidOperationException();

            Contract.Assert(!capabilities.CanSuspend || rawInterface.SuspendThread != null);
            Contract.Assert(!capabilities.CanSuspend || rawInterface.SuspendThreadList != null);
            Contract.Assert(!capabilities.CanSuspend || rawInterface.ResumeThread != null);
            Contract.Assert(!capabilities.CanSuspend || rawInterface.ResumeThreadList != null);

            Contract.Assert(!capabilities.CanSignalThread || rawInterface.StopThread != null);
            Contract.Assert(!capabilities.CanSignalThread || rawInterface.InterruptThread != null);

            Contract.Assert(!capabilities.CanGetOwnedMonitorInfo || rawInterface.GetOwnedMonitorInfo != null);

            Contract.Assert(!capabilities.CanGetOwnedMonitorStackDepthInfo || rawInterface.GetOwnedMonitorStackDepthInfo != null);
            Contract.Assert(!capabilities.CanGetCurrentContendedMonitor || rawInterface.GetCurrentContendedMonitor != null);
            Contract.Assert(!capabilities.CanPopFrame || rawInterface.PopFrame != null);
            Contract.Assert(!capabilities.CanGenerateFramePopEvents || rawInterface.NotifyFramePop != null);
            Contract.Assert(!capabilities.CanForceEarlyReturn || rawInterface.ForceEarlyReturnObject != null);
            Contract.Assert(!capabilities.CanForceEarlyReturn || rawInterface.ForceEarlyReturnInt != null);
            Contract.Assert(!capabilities.CanForceEarlyReturn || rawInterface.ForceEarlyReturnLong != null);
            Contract.Assert(!capabilities.CanForceEarlyReturn || rawInterface.ForceEarlyReturnFloat != null);
            Contract.Assert(!capabilities.CanForceEarlyReturn || rawInterface.ForceEarlyReturnDouble != null);
            Contract.Assert(!capabilities.CanForceEarlyReturn || rawInterface.ForceEarlyReturnVoid != null);
            Contract.Assert(!capabilities.CanTagObjects || rawInterface.FollowReferences != null);
            Contract.Assert(!capabilities.CanTagObjects || rawInterface.IterateThroughHeap != null);
            Contract.Assert(!capabilities.CanTagObjects || rawInterface.GetTag != null);
            Contract.Assert(!capabilities.CanTagObjects || rawInterface.SetTag != null);
            Contract.Assert(!capabilities.CanTagObjects || rawInterface.GetObjectsWithTags != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.GetLocalObject != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.GetLocalInstance != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.GetLocalInt != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.GetLocalLong != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.GetLocalFloat != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.GetLocalDouble != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.SetLocalObject != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.SetLocalInt != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.SetLocalLong != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.SetLocalFloat != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.SetLocalDouble != null);
            Contract.Assert(!capabilities.CanGenerateBreakpointEvents || rawInterface.SetBreakpoint != null);
            Contract.Assert(!capabilities.CanGenerateBreakpointEvents || rawInterface.ClearBreakpoint != null);
            Contract.Assert(!capabilities.CanGenerateFieldAccessEvents || rawInterface.SetFieldAccessWatch != null);
            Contract.Assert(!capabilities.CanGenerateFieldAccessEvents || rawInterface.ClearFieldAccessWatch != null);
            Contract.Assert(!capabilities.CanGenerateFieldModificationEvents || rawInterface.SetFieldModificationWatch != null);
            Contract.Assert(!capabilities.CanGenerateFieldModificationEvents || rawInterface.ClearFieldModificationWatch != null);

            Contract.Assert(!capabilities.CanGetSourceFileName || rawInterface.GetSourceFileName != null);

            Contract.Assert(!capabilities.CanGetConstantPool || rawInterface.GetConstantPool != null);

            Contract.Assert(!capabilities.CanGetSourceDebugExtension || rawInterface.GetSourceDebugExtension != null);
            Contract.Assert(!capabilities.CanRetransformClasses || rawInterface.RetransformClasses != null);
            Contract.Assert(!capabilities.CanRedefineClasses || rawInterface.RedefineClasses != null);

            Contract.Assert(!capabilities.CanGetMonitorInfo || rawInterface.GetObjectMonitorUsage != null);

            Contract.Assert(!capabilities.CanGetSyntheticAttribute || rawInterface.IsFieldSynthetic != null);
            Contract.Assert(!capabilities.CanGetLineNumbers || rawInterface.GetLineNumberTable != null);
            Contract.Assert(!capabilities.CanAccessLocalVariables || rawInterface.GetLocalVariableTable != null);
            Contract.Assert(!capabilities.CanGetBytecodes || rawInterface.GetBytecodes != null);
            Contract.Assert(!capabilities.CanGetSyntheticAttribute || rawInterface.IsMethodSynthetic != null);
            Contract.Assert(!capabilities.CanSetNativeMethodPrefix || rawInterface.SetNativeMethodPrefix != null);
            Contract.Assert(!capabilities.CanSetNativeMethodPrefix || rawInterface.SetNativeMethodPrefixes != null);

            Contract.Assert(!capabilities.CanGetCurrentThreadCpuTime || rawInterface.GetCurrentThreadCpuTimerInfo != null);
            Contract.Assert(!capabilities.CanGetCurrentThreadCpuTime || rawInterface.GetCurrentThreadCpuTime != null);
            Contract.Assert(!capabilities.CanGetThreadCpuTime || rawInterface.GetCurrentThreadCpuTimerInfo != null);
            Contract.Assert(!capabilities.CanGetThreadCpuTime || rawInterface.GetCurrentThreadCpuTime != null);

            _env = env;
            _rawInterface = rawInterface;

            _eventManager = new JvmEventManager(this);
            _eventManager.Attach();
        }

        internal static JvmEnvironment GetOrCreateEnvironment(jvmtiEnvHandle env)
        {
            return _environments.GetOrAdd(env, CreateEnvironment);
        }

        private static JvmEnvironment CreateEnvironment(jvmtiEnvHandle env)
        {
            return new JvmEnvironment(env);
        }

        #region Memory Management

        public SafeJvmAllocHandle Allocate(long size)
        {
            IntPtr result;
            ThrowOnFailure(_rawInterface.Allocate(_env, size, out result));
            return new SafeJvmAllocHandle(this, result, true);
        }

        internal void Deallocate(IntPtr handle)
        {
            _rawInterface.Deallocate(_env, handle);
        }

        #endregion

        #region Thread

        internal jvmtiThreadState GetThreadState(JvmThreadReference thread)
        {
            jvmtiThreadState threadState;
            ThrowOnFailure(_rawInterface.GetThreadState(_env, (jthread)thread, out threadState));
            return threadState;
        }

        public JvmThreadReference GetCurrentThread(JvmNativeEnvironment nativeEnvironment)
        {
            jthread threadHandle;
            ThrowOnFailure(_rawInterface.GetCurrentThread(_env, out threadHandle));
            return new JvmThreadReference(this, nativeEnvironment, threadHandle);
        }

        public DisposableObjectCollection<JvmThreadReference> GetAllThreads(JvmNativeEnvironment nativeEnvironment)
        {
            DisposableObjectCollection<JvmThreadReference> result = new DisposableObjectCollection<JvmThreadReference>();

            int threadsCount;
            IntPtr threads;
            ThrowOnFailure(_rawInterface.GetAllThreads(_env, out threadsCount, out threads));
            try
            {
                unsafe
                {
                    jthread* rawThreads = (jthread*)threads;
                    for (int i = 0; i < threadsCount; i++)
                    {
                        result.Add(new JvmThreadReference(this, nativeEnvironment, rawThreads[i]));
                    }
                }

                return result;
            }
            finally
            {
                if (threads != IntPtr.Zero)
                    Deallocate(threads);
            }
        }

        public int GetSuspendCount(JvmThreadReference thread)
        {
            throw new NotImplementedException();
        }

        public void SuspendThread(JvmThreadReference thread)
        {
            throw new NotImplementedException();
        }

        public void SuspendThreads(IEnumerable<JvmThreadReference> threads)
        {
            throw new NotImplementedException();
        }

        public void ResumeThread(JvmThreadReference thread)
        {
            throw new NotImplementedException();
        }

        public void ResumeThreads(IEnumerable<JvmThreadReference> threads)
        {
            throw new NotImplementedException();
        }

        public void StopThread(JvmThreadReference thread, JvmObjectReference exception)
        {
            throw new NotImplementedException();
        }

        public void InterruptThread(JvmThreadReference thread)
        {
            throw new NotImplementedException();
        }

        internal jvmtiThreadInfo GetThreadInfo(JvmThreadReference thread)
        {
            jvmtiThreadInfo info;
            ThrowOnFailure(_rawInterface.GetThreadInfo(_env, (jthread)thread, out info));
            return info;
        }

        public DisposableObjectCollection<JvmObjectReference> GetOwnedMonitorInfo(JvmThreadReference thread, JvmNativeEnvironment nativeEnvironment)
        {
            DisposableObjectCollection<JvmObjectReference> result = new DisposableObjectCollection<JvmObjectReference>();

            int ownedMonitorCount;
            IntPtr ownedMonitors;
            ThrowOnFailure(_rawInterface.GetOwnedMonitorInfo(_env, (jthread)thread, out ownedMonitorCount, out ownedMonitors));
            try
            {
                unsafe
                {
                    jobject* rawMonitors = (jobject*)ownedMonitors;
                    for (int i = 0; i < ownedMonitorCount; i++)
                    {
                        result.Add(new JvmObjectReference(this, nativeEnvironment, rawMonitors[i]));
                    }
                }

                return result;
            }
            finally
            {
                if (ownedMonitors != IntPtr.Zero)
                    Deallocate(ownedMonitors);
            }
        }

        public DisposableObjectCollection<JvmMonitorInfo> GetOwnedMonitorStackDepthInfo(JvmThreadReference thread)
        {
            throw new NotImplementedException();
        }

        public JvmObjectReference GetCurrentContendedMonitor(JvmThreadReference thread)
        {
            throw new NotImplementedException();
        }

        internal void RunAgentThread(JvmThreadReference thread, UnsafeNativeMethods.jvmtiStartFunction proc, IntPtr arg, int priority)
        {
            throw new NotImplementedException();
        }

        public void SetThreadLocalStorage(JvmThreadReference thread, IntPtr data)
        {
            ThrowOnFailure(_rawInterface.SetThreadLocalStorage(_env, (jthread)thread, data));
        }

        public IntPtr GetThreadLocalStorage(JvmThreadReference thread)
        {
            IntPtr data;
            ThrowOnFailure(_rawInterface.GetThreadLocalStorage(_env, (jthread)thread, out data));
            return data;
        }

        #endregion

        #region Thread Group

        public DisposableObjectCollection<JvmThreadGroupReference> GetTopThreadGroups()
        {
            throw new NotImplementedException();
        }

        internal jvmtiThreadGroupInfo GetThreadGroupInfo(JvmThreadGroupReference group)
        {
            throw new NotImplementedException();
        }

        public void GetThreadGroupChildren(JvmThreadGroupReference group, IList<JvmThreadReference> threads, IList<JvmThreadGroupReference> groups)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Stack Frame

        public IList<JvmLocation> GetStackTrace(JvmThreadReference thread, int startDepth, int maxFrameCount)
        {
            throw new NotImplementedException();
        }

        public DisposableObjectCollection<JvmStackInfo> GetAllStackTraces(int maxFrameCount)
        {
            throw new NotImplementedException();
        }

        public DisposableObjectCollection<JvmStackInfo> GetThreadListStackTraces(IEnumerable<JvmThreadReference> threads, int maxFrameCount)
        {
            throw new NotImplementedException();
        }

        public int GetFrameCount(JvmThreadReference thread)
        {
            throw new NotImplementedException();
        }

        public void PopFrame(JvmThreadReference thread)
        {
            throw new NotImplementedException();
        }

        public JvmLocation GetFrameLocation(JvmThreadReference thread, int depth)
        {
            throw new NotImplementedException();
        }

        public void NotifyFramePop(JvmThreadReference thread, int depth)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Force Early Return

        #endregion

        #region Heap

        #endregion

        #region Local Variable

        #endregion

        #region Breakpoint

        #endregion

        #region Watched Field

        #endregion

        #region Class

        #endregion

        #region Object

        #endregion

        #region Field

        #endregion

        #region Method

        public void GetMethodName(JvmMethod method, out string name, out string signature, out string generic)
        {
            IntPtr namePtr;
            IntPtr signaturePtr;
            IntPtr genericPtr;
            ThrowOnFailure(_rawInterface.GetMethodName(_env, (jmethodID)method, out namePtr, out signaturePtr, out genericPtr));
            try
            {
                name = null;
                signature = null;
                generic = null;
                unsafe
                {
                    if (namePtr != IntPtr.Zero)
                        name = ModifiedUTF8Encoding.GetString((byte*)namePtr);
                    if (signaturePtr != IntPtr.Zero)
                        signature = ModifiedUTF8Encoding.GetString((byte*)signaturePtr);
                    if (genericPtr != IntPtr.Zero)
                        generic = ModifiedUTF8Encoding.GetString((byte*)genericPtr);
                }
            }
            finally
            {
                Deallocate(namePtr);
                Deallocate(signaturePtr);
                Deallocate(genericPtr);
            }
        }

        public JvmClassReference GetMethodDeclaringClass(JvmMethod method)
        {
            throw new NotImplementedException();
        }

        public JvmAccessModifiers GetMethodModifiers(JvmMethod method)
        {
            JvmAccessModifiers modifiers;
            ThrowOnFailure(_rawInterface.GetMethodModifiers(_env, (jmethodID)method, out modifiers));
            return modifiers;
        }

        public int GetMaxLocals(JvmMethod method)
        {
            int maxLocals;
            ThrowOnFailure(_rawInterface.GetMaxLocals(_env, (jmethodID)method, out maxLocals));
            return maxLocals;
        }

        public int GetArgumentsSize(JvmMethod method)
        {
            int argumentsSize;
            ThrowOnFailure(_rawInterface.GetArgumentsSize(_env, (jmethodID)method, out argumentsSize));
            return argumentsSize;
        }

        public bool IsMethodNative(JvmMethod method)
        {
            byte isNative;
            ThrowOnFailure(_rawInterface.IsMethodNative(_env, (jmethodID)method, out isNative));
            return isNative != 0;
        }

        public bool IsMethodSynthetic(JvmMethod method)
        {
            byte isSynthetic;
            ThrowOnFailure(_rawInterface.IsMethodSynthetic(_env, (jmethodID)method, out isSynthetic));
            return isSynthetic != 0;
        }

        public bool IsMethodObsolete(JvmMethod method)
        {
            byte isObsolete;
            ThrowOnFailure(_rawInterface.IsMethodObsolete(_env, (jmethodID)method, out isObsolete));
            return isObsolete != 0;
        }

        #endregion

        #region Raw Monitor

        #endregion

        #region JNI Function Interception

        internal JvmNativeEnvironment GetNativeFunctionTable(JNIEnvHandle nativeEnvironmentHandle)
        {
            IntPtr functionTable;
            ThrowOnFailure(_rawInterface.GetJNIFunctionTable(_env, out functionTable));
            try
            {
                jniNativeInterface nativeInterface = (jniNativeInterface)Marshal.PtrToStructure(functionTable, typeof(jniNativeInterface));
                return new JvmNativeEnvironment(this, nativeEnvironmentHandle, nativeInterface);

            }
            finally
            {
                if (functionTable != IntPtr.Zero)
                    Deallocate(functionTable);
            }
        }

        #endregion

        #region Event Management

        internal void SetEventCallbacks(jvmtiEventCallbacks callbacks)
        {
            int sizeOfCallbacks = Marshal.SizeOf(callbacks);
            ThrowOnFailure(_rawInterface.SetEventCallbacks(_env, ref callbacks, sizeOfCallbacks));
        }

        public void SetEventNotificationMode(JvmEventMode mode, JvmEventType eventType)
        {
            SetEventNotificationMode(mode, eventType, default(JvmThreadReference));
        }

        public void SetEventNotificationMode(JvmEventMode mode, JvmEventType eventType, JvmThreadReference eventThread)
        {
            jthread eventThreadHandle = (jthread)eventThread;
            ThrowOnFailure(_rawInterface.SetEventNotificationMode(_env, mode, eventType, eventThreadHandle));
        }

        public void GenerateEvents(JvmEventType eventType)
        {
            ThrowOnFailure(_rawInterface.GenerateEvents(_env, eventType));
        }

        #endregion

        #region Extension Mechanism

        #endregion

        #region Capability

        public jvmtiCapabilities GetPotentialCapabilities()
        {
            jvmtiCapabilities capabilities;
            ThrowOnFailure(_rawInterface.GetPotentialCapabilities(_env, out capabilities));
            return capabilities;
        }

        public void AddCapabilities(jvmtiCapabilities capabilities)
        {
            ThrowOnFailure(_rawInterface.AddCapabilities(_env, ref capabilities));
        }

        public void RelinquishCapabilities(jvmtiCapabilities capabilities)
        {
            ThrowOnFailure(_rawInterface.AddCapabilities(_env, ref capabilities));
        }

        public jvmtiCapabilities GetCapabilities()
        {
            jvmtiCapabilities capabilities;
            ThrowOnFailure(_rawInterface.GetCapabilities(_env, out capabilities));
            return capabilities;
        }

        #endregion

        #region Timers

        #endregion

        #region Class Loader Search

        #endregion

        #region System Properties

        #endregion

        #region General

        #endregion

        private void ThrowOnFailure(jvmtiError result)
        {
            if (result == jvmtiError.None)
                return;

            IntPtr namePtr;
            string message;
            if (_rawInterface.GetErrorName(_env, result, out namePtr) == jvmtiError.None)
            {
                unsafe
                {
                    message = ModifiedUTF8Encoding.GetString((byte*)namePtr);
                }
            }
            else
            {
                message = result.ToString();
            }

            throw new InvalidOperationException(message);
        }
    }
}
