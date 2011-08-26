namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugHost.Interop;
    using Marshal = System.Runtime.InteropServices.Marshal;
    using DispatcherFrame = System.Windows.Threading.DispatcherFrame;

    internal class JvmEventManager
    {
        private readonly JvmEnvironment _environment;
        private readonly jvmtiEventCallbacks _eventCallbacks;
        private readonly List<JvmEventProcessorBase> _processors = new List<JvmEventProcessorBase>();

        internal JvmEventManager(JvmEnvironment environment)
        {
            Contract.Requires<ArgumentNullException>(environment != null, "environment");

            _environment = environment;

            _eventCallbacks = new jvmtiEventCallbacks();
            _eventCallbacks.VMInit = HandleVMInit;
            _eventCallbacks.VMDeath = HandleVMDeath;
            _eventCallbacks.ThreadStart = HandleThreadStart;
            _eventCallbacks.ThreadEnd = HandleThreadEnd;
            _eventCallbacks.ClassFileLoadHook = HandleClassFileLoadHook;
            _eventCallbacks.ClassLoad = HandleClassLoad;
            _eventCallbacks.ClassPrepare = HandleClassPrepare;
            _eventCallbacks.VMStart = HandleVMStart;
            _eventCallbacks.Exception = HandleException;
            _eventCallbacks.ExceptionCatch = HandleExceptionCatch;
            _eventCallbacks.SingleStep = HandleSingleStep;
            _eventCallbacks.FramePop = HandleFramePop;
            _eventCallbacks.Breakpoint = HandleBreakpoint;
            _eventCallbacks.FieldAccess = HandleFieldAccess;
            _eventCallbacks.FieldModification = HandleFieldModification;
            _eventCallbacks.MethodEntry = HandleMethodEntry;
            _eventCallbacks.MethodExit = HandleMethodExit;
            _eventCallbacks.NativeMethodBind = HandleNativeMethodBind;
            _eventCallbacks.CompiledMethodLoad = HandleCompiledMethodLoad;
            _eventCallbacks.CompiledMethodUnload = HandleCompiledMethodUnload;
            _eventCallbacks.DynamicCodeGenerated = HandleDynamicCodeGenerated;
            _eventCallbacks.DataDumpRequest = HandleDataDumpRequest;
            _eventCallbacks.MonitorWait = HandleMonitorWait;
            _eventCallbacks.MonitorWaited = HandleMonitorWaited;
            _eventCallbacks.MonitorContendedEnter = HandleMonitorContendedEnter;
            _eventCallbacks.MonitorContendedEntered = HandleMonitorContendedEntered;
            _eventCallbacks.ResourceExhausted = HandleResourceExhausted;
            _eventCallbacks.GarbageCollectionStart = HandleGarbageCollectionStart;
            _eventCallbacks.GarbageCollectionFinish = HandleGarbageCollectionFinish;
            _eventCallbacks.ObjectFree = HandleObjectFree;
            _eventCallbacks.VMObjectAlloc = HandleVMObjectAlloc;
        }

        public void AddProcessor(JvmEventProcessorBase processor)
        {
            Contract.Requires<ArgumentNullException>(processor != null, "processor");
            Contract.Assert(_processors.Count == 0);
            _processors.Add(processor);
        }

        public void RemoveProcessor(JvmEventProcessorBase processor)
        {
            Contract.Requires<ArgumentNullException>(processor != null, "processor");
            _processors.Remove(processor);
        }

        internal void Attach()
        {
            _environment.SetEventCallbacks(_eventCallbacks);

            jvmtiCapabilities capabilities = _environment.GetCapabilities();

            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMStart);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMInit);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMDeath);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ThreadStart);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ThreadEnd);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ClassLoad);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ClassPrepare);
#if false
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ClassFileLoadHook);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.DynamicCodeGenerated);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.DataDumpRequest);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ResourceExhausted);
            //_environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.DataResetRequest);

            if (capabilities.CanGenerateExceptionEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.Exception);
            if (capabilities.CanGenerateExceptionEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ExceptionCatch);
            if (capabilities.CanGenerateSingleStepEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.SingleStep);
            if (capabilities.CanGenerateFramePopEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.FramePop);
            if (capabilities.CanGenerateBreakpointEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.Breakpoint);
            if (capabilities.CanGenerateFieldAccessEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.FieldAccess);
            if (capabilities.CanGenerateFieldModificationEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.FieldModification);
            if (capabilities.CanGenerateMethodEntryEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.MethodEntry);
            if (capabilities.CanGenerateMethodExitEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.MethodExit);
            if (capabilities.CanGenerateNativeMethodBindEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.NativeMethodBind);
            if (capabilities.CanGenerateCompiledMethodLoadEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.CompiledMethodLoad);
            if (capabilities.CanGenerateCompiledMethodLoadEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.CompiledMethodUnload);
            if (capabilities.CanGenerateMonitorEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.MonitorWait);
            if (capabilities.CanGenerateMonitorEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.MonitorWaited);
            if (capabilities.CanGenerateMonitorEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.MonitorContendedEnter);
            if (capabilities.CanGenerateMonitorEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.MonitorContendedEntered);
            if (capabilities.CanGenerateGarbageCollectionEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.GarbageCollectionStart);
            if (capabilities.CanGenerateGarbageCollectionEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.GarbageCollectionFinish);
            if (capabilities.CanGenerateObjectFreeEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ObjectFree);
            if (capabilities.CanGenerateVmObjectAllocEvents)
                _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMObjectAlloc);
#endif
        }

        internal void Detach()
        {
            _environment.SetEventCallbacks(default(jvmtiEventCallbacks));
        }

        private void HandleVMInit(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);

            jvmtiError result = environment.RawInterface.RunAgentThread(env, alloc_thread(jniEnv), DispatchJvmEvents, IntPtr.Zero, JvmThreadPriority.Maximum);

            foreach (var processor in _processors)
            {
                processor.HandleVMInitialization(environment, thread);
            }
        }

        private static jthread alloc_thread(JNIEnvHandle jniEnv)
        {
            jniNativeInterface nativeInterface = (jniNativeInterface)Marshal.PtrToStructure(Marshal.ReadIntPtr(jniEnv.Handle), typeof(jniNativeInterface));

            jclass @class = nativeInterface.FindClass(jniEnv, "java/lang/Thread");
            if (@class == jclass.Null)
                throw new Exception("ERROR: JNI: Cannot find %s with FindClass.");

            nativeInterface.ExceptionClear(jniEnv);

            jmethodID method = nativeInterface.GetMethodID(jniEnv, @class, "<init>", "()V");
            if (method == jmethodID.Null)
                throw new Exception("Cannot find Thread constructor method.");

            nativeInterface.ExceptionClear(jniEnv);
            jthread result = (jthread)nativeInterface.NewObject(jniEnv, @class, method);
            if (result == jthread.Null)
                throw new Exception("Cannot create new Thread object");

            nativeInterface.ExceptionClear(jniEnv);
            return result;
        }

        private static void DispatchJvmEvents(jvmtiEnvHandle env, JNIEnvHandle jniEnv, IntPtr arg)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            environment.VirtualMachine.PushAgentDispatcherFrame(new DispatcherFrame(true), environment);
        }

        private void HandleVMDeath(jvmtiEnvHandle env, JNIEnvHandle jniEnv)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleVMDeath(environment);
            }

            environment.VirtualMachine.ShutdownAgentDispatchers();
        }

        private void HandleThreadStart(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleThreadStart(environment, thread);
            }
        }

        private void HandleThreadEnd(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleThreadEnd(environment, thread);
            }
        }

        private void HandleClassFileLoadHook(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jclass classBeingRedefinedHandle, jobject loaderHandle, ModifiedUTF8StringData name, jobject protectionDomainHandle, int classDataLength, IntPtr classData, ref int newClassDataLength, ref IntPtr newClassData)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmClassReference classBeingRedefined = JvmClassReference.FromHandle(environment, jniEnv, classBeingRedefinedHandle, true);
            JvmObjectReference loader = JvmObjectReference.FromHandle(environment, jniEnv, loaderHandle, true);
            JvmObjectReference protectionDomain = JvmObjectReference.FromHandle(environment, jniEnv, protectionDomainHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleClassFileLoadHook(environment, classBeingRedefined, loader, name.GetString(), protectionDomain);
            }
        }

        private void HandleClassLoad(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jclass classHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmClassReference @class = JvmClassReference.FromHandle(environment, jniEnv, classHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleClassLoad(environment, thread, @class);
            }
        }

        private void HandleClassPrepare(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jclass classHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmClassReference @class = JvmClassReference.FromHandle(environment, jniEnv, classHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleClassPrepare(environment, thread, @class);
            }
        }

        private void HandleVMStart(jvmtiEnvHandle env, JNIEnvHandle jniEnv)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleVMStart(environment);
            }
        }

        private void HandleException(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation, jobject exceptionHandle, jmethodID catchMethod, jlocation catchjLocation)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmLocation location = new JvmLocation(environment, method, jlocation);
            JvmObjectReference exception = JvmObjectReference.FromHandle(environment, jniEnv, exceptionHandle, true);
            JvmLocation catchLocation = new JvmLocation(environment, catchMethod, catchjLocation);

            foreach (var processor in _processors)
            {
                processor.HandleException(environment, thread, location, exception, catchLocation);
            }
        }

        private void HandleExceptionCatch(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation, jobject exceptionHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmLocation location = new JvmLocation(environment, method, jlocation);
            JvmObjectReference exception = JvmObjectReference.FromHandle(environment, jniEnv, exceptionHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleExceptionCatch(environment, thread, location, exception);
            }
        }

        private void HandleSingleStep(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmLocation location = new JvmLocation(environment, method, jlocation);

            foreach (var processor in _processors)
            {
                processor.HandleSingleStep(environment, thread, location);
            }
        }

        private void HandleFramePop(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, bool wasPoppedByException)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                processor.HandleFramePop(environment, thread, method, wasPoppedByException);
            }
        }

        private void HandleBreakpoint(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmLocation location = new JvmLocation(environment, method, jlocation);

            foreach (var processor in _processors)
            {
                processor.HandleBreakpoint(environment, thread, location);
            }
        }

        private void HandleFieldAccess(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation, jclass fieldClassHandle, jobject objectHandle, jfieldID fieldId)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmLocation location = new JvmLocation(environment, method, jlocation);
            JvmClassReference fieldClass = JvmClassReference.FromHandle(environment, jniEnv, fieldClassHandle, true);
            JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);
            JvmField field = new JvmField(environment, fieldId);

            foreach (var processor in _processors)
            {
                processor.HandleFieldAccess(environment, thread, location, fieldClass, @object, field);
            }
        }

        private void HandleFieldModification(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation, jclass fieldClassHandle, jobject @objectHandle, jfieldID fieldId, byte signatureType, jvalue newValue)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmLocation location = new JvmLocation(environment, method, jlocation);
            JvmClassReference fieldClass = JvmClassReference.FromHandle(environment, jniEnv, fieldClassHandle, true);
            JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);
            JvmField field = new JvmField(environment, fieldId);

            foreach (var processor in _processors)
            {
                processor.HandleFieldModification(environment, thread, location, fieldClass, @object, field, signatureType, newValue);
            }
        }

        private void HandleMethodEntry(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                processor.HandleMethodEntry(environment, thread, method);
            }
        }

        private void HandleMethodExit(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, bool wasPoppedByException, jvalue returnValue)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                processor.HandleMethodExit(environment, thread, method, wasPoppedByException, returnValue);
            }
        }

        private void HandleNativeMethodBind(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, IntPtr address, ref IntPtr newAddressPtr)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                IntPtr? newAddress = null;
                processor.HandleNativeMethodBind(environment, thread, method, address, ref newAddress);
            }
        }

        private void HandleCompiledMethodLoad(jvmtiEnvHandle env, jmethodID method, int codeSize, IntPtr codeAddress, int mapLength, jvmtiAddressLocationMap[] map, IntPtr compileInfo)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            foreach (var processor in _processors)
            {
                throw new NotImplementedException();
                //processor.HandleCompiledMethodLoad(environment, method, codeSize, codeAddress, map2, compileInfo);
            }
        }

        private void HandleCompiledMethodUnload(jvmtiEnvHandle env, jmethodID methodId, IntPtr codeAddress)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                processor.HandleCompiledMethodUnload(environment, method, codeAddress);
            }
        }

        private void HandleDynamicCodeGenerated(jvmtiEnvHandle env, ModifiedUTF8StringData name, IntPtr address, int length)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleDynamicCodeGenerated(environment, name.GetString(), address, length);
            }
        }

        private void HandleDataDumpRequest(jvmtiEnvHandle env)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleDataDumpRequest(environment);
            }
        }

        private void HandleMonitorWait(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle, long millisecondsTimeout)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleMonitorWait(environment, thread, @object, millisecondsTimeout);
            }
        }

        private void HandleMonitorWaited(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle, bool timedOut)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleMonitorWaited(environment, thread, @object, timedOut);
            }
        }

        private void HandleMonitorContendedEnter(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleMonitorContendedEnter(environment, thread, @object);
            }
        }

        private void HandleMonitorContendedEntered(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleMonitorContendedEntered(environment, thread, @object);
            }
        }

        private void HandleResourceExhausted(jvmtiEnvHandle env, JNIEnvHandle jniEnv, JvmResourceExhaustedFlags flags, IntPtr reserved, ModifiedUTF8StringData description)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleResourceExhausted(environment, flags, reserved, description.GetString());
            }
        }

        private void HandleGarbageCollectionStart(jvmtiEnvHandle env)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleGarbageCollectionStart(environment);
            }
        }

        private void HandleGarbageCollectionFinish(jvmtiEnvHandle env)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleGarbageCollectionFinish(environment);
            }
        }

        private void HandleObjectFree(jvmtiEnvHandle env, long tag)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleObjectFree(environment, tag);
            }
        }

        private void HandleVMObjectAlloc(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle, jclass objectClassHandle, long size)
        {
            JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
            JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
            JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);
            JvmClassReference objectClass = JvmClassReference.FromHandle(environment, jniEnv, objectClassHandle, true);

            foreach (var processor in _processors)
            {
                processor.HandleVMObjectAllocation(environment, thread, @object, objectClass, size);
            }
        }
    }
}
