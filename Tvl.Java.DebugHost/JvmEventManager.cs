namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugHost.Interop;
    using jlocation = System.Int64;
    using jvalue = System.Int64;

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

        internal void Attach()
        {
            _environment.SetEventCallbacks(_eventCallbacks);

            jvmtiCapabilities capabilities = _environment.GetCapabilities();

            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMInit);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMDeath);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ThreadStart);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ThreadEnd);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ClassFileLoadHook);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ClassLoad);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ClassPrepare);
            _environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMStart);
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
        }

        internal void Detach()
        {
            _environment.SetEventCallbacks(default(jvmtiEventCallbacks));
        }

        private void HandleVMInit(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);

            foreach (var processor in _processors)
            {
                processor.HandleVMInitialization(environment, nativeEnvironment, thread);
            }
        }

        private void HandleVMDeath(jvmtiEnvHandle env, JNIEnvHandle jniEnv)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);

            foreach (var processor in _processors)
            {
                processor.HandleVMDeath(environment, nativeEnvironment);
            }
        }

        private void HandleThreadStart(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);

            foreach (var processor in _processors)
            {
                processor.HandleThreadStart(environment, nativeEnvironment, thread);
            }
        }

        private void HandleThreadEnd(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);

            foreach (var processor in _processors)
            {
                processor.HandleThreadEnd(environment, nativeEnvironment, thread);
            }
        }

        private void HandleClassFileLoadHook(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jclass classBeingRedefinedHandle, jobject loaderHandle, ModifiedUTF8StringData name, jobject protectionDomainHandle, int classDataLength, IntPtr classData, ref int newClassDataLength, ref IntPtr newClassData)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmClassReference classBeingRedefined = new JvmClassReference(environment, nativeEnvironment, classBeingRedefinedHandle);
            JvmObjectReference loader = new JvmObjectReference(environment, nativeEnvironment, loaderHandle);
            JvmObjectReference protectionDomain = new JvmObjectReference(environment, nativeEnvironment, protectionDomainHandle);

            foreach (var processor in _processors)
            {
                throw new NotImplementedException();
                //processor.HandleClassFileLoadHook(environment, nativeEnvironment, classBeingRedefined, loader, name.GetString(), protectionDomain, classData, ref classData);
            }
        }

        private void HandleClassLoad(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jclass classHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmClassReference @class = new JvmClassReference(environment, nativeEnvironment, classHandle);

            foreach (var processor in _processors)
            {
                processor.HandleClassLoad(environment, nativeEnvironment, thread, @class);
            }
        }

        private void HandleClassPrepare(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jclass classHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmClassReference @class = new JvmClassReference(environment, nativeEnvironment, classHandle);

            foreach (var processor in _processors)
            {
                processor.HandleClassPrepare(environment, nativeEnvironment, thread, @class);
            }
        }

        private void HandleVMStart(jvmtiEnvHandle env, JNIEnvHandle jniEnv)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);

            foreach (var processor in _processors)
            {
                processor.HandleVMStart(environment, nativeEnvironment);
            }
        }

        private void HandleException(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation, jobject exceptionHandle, jmethodID catchMethod, jlocation catchjLocation)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmLocation location = new JvmLocation(environment, method, jlocation);
            JvmObjectReference exception = new JvmObjectReference(environment, nativeEnvironment, exceptionHandle);
            JvmLocation catchLocation = new JvmLocation(environment, catchMethod, catchjLocation);

            foreach (var processor in _processors)
            {
                processor.HandleException(environment, nativeEnvironment, thread, location, exception, catchLocation);
            }
        }

        private void HandleExceptionCatch(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation, jobject exceptionHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmLocation location = new JvmLocation(environment, method, jlocation);
            JvmObjectReference exception = new JvmObjectReference(environment, nativeEnvironment, exceptionHandle);

            foreach (var processor in _processors)
            {
                processor.HandleExceptionCatch(environment, nativeEnvironment, thread, location, exception);
            }
        }

        private void HandleSingleStep(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmLocation location = new JvmLocation(environment, method, jlocation);

            foreach (var processor in _processors)
            {
                processor.HandleSingleStep(environment, nativeEnvironment, thread, location);
            }
        }

        private void HandleFramePop(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, bool wasPoppedByException)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                processor.HandleFramePop(environment, nativeEnvironment, thread, method, wasPoppedByException);
            }
        }

        private void HandleBreakpoint(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmLocation location = new JvmLocation(environment, method, jlocation);

            foreach (var processor in _processors)
            {
                processor.HandleBreakpoint(environment, nativeEnvironment, thread, location);
            }
        }

        private void HandleFieldAccess(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation, jclass fieldClassHandle, jobject objectHandle, jfieldID fieldId)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmLocation location = new JvmLocation(environment, method, jlocation);
            JvmClassReference fieldClass = new JvmClassReference(environment, nativeEnvironment, fieldClassHandle);
            JvmObjectReference @object = new JvmObjectReference(environment, nativeEnvironment, objectHandle);
            JvmField field = new JvmField(environment, fieldId);

            foreach (var processor in _processors)
            {
                processor.HandleFieldAccess(environment, nativeEnvironment, thread, location, fieldClass, @object, field);
            }
        }

        private void HandleFieldModification(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID method, jlocation jlocation, jclass fieldClassHandle, jobject @objectHandle, jfieldID fieldId, byte signatureType, jvalue newValue)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmLocation location = new JvmLocation(environment, method, jlocation);
            JvmClassReference fieldClass = new JvmClassReference(environment, nativeEnvironment, fieldClassHandle);
            JvmObjectReference @object = new JvmObjectReference(environment, nativeEnvironment, objectHandle);
            JvmField field = new JvmField(environment, fieldId);

            foreach (var processor in _processors)
            {
                processor.HandleFieldModification(environment, nativeEnvironment, thread, location, fieldClass, @object, field, signatureType, newValue);
            }
        }

        private void HandleMethodEntry(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                processor.HandleMethodEntry(environment, nativeEnvironment, thread, method);
            }
        }

        private void HandleMethodExit(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, bool wasPoppedByException, jvalue returnValue)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                processor.HandleMethodExit(environment, nativeEnvironment, thread, method, wasPoppedByException, returnValue);
            }
        }

        private void HandleNativeMethodBind(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, IntPtr address, ref IntPtr newAddressPtr)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                IntPtr? newAddress = null;
                processor.HandleNativeMethodBind(environment, nativeEnvironment, thread, method, address, ref newAddress);
            }
        }

        private void HandleCompiledMethodLoad(jvmtiEnvHandle env, jmethodID method, int codeSize, IntPtr codeAddress, int mapLength, jvmtiAddressLocationMap[] map, IntPtr compileInfo)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);

            foreach (var processor in _processors)
            {
                throw new NotImplementedException();
                //processor.HandleCompiledMethodLoad(environment, method, codeSize, codeAddress, map2, compileInfo);
            }
        }

        private void HandleCompiledMethodUnload(jvmtiEnvHandle env, jmethodID methodId, IntPtr codeAddress)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmMethod method = new JvmMethod(environment, methodId);

            foreach (var processor in _processors)
            {
                processor.HandleCompiledMethodUnload(environment, method, codeAddress);
            }
        }

        private void HandleDynamicCodeGenerated(jvmtiEnvHandle env, ModifiedUTF8StringData name, IntPtr address, int length)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleDynamicCodeGenerated(environment, name.GetString(), address, length);
            }
        }

        private void HandleDataDumpRequest(jvmtiEnvHandle env)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleDataDumpRequest(environment);
            }
        }

        private void HandleMonitorWait(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle, long millisecondsTimeout)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmObjectReference @object = new JvmObjectReference(environment, nativeEnvironment, objectHandle);

            foreach (var processor in _processors)
            {
                processor.HandleMonitorWait(environment, nativeEnvironment, thread, @object, millisecondsTimeout);
            }
        }

        private void HandleMonitorWaited(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle, bool timedOut)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmObjectReference @object = new JvmObjectReference(environment, nativeEnvironment, objectHandle);

            foreach (var processor in _processors)
            {
                processor.HandleMonitorWaited(environment, nativeEnvironment, thread, @object, timedOut);
            }
        }

        private void HandleMonitorContendedEnter(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmObjectReference @object = new JvmObjectReference(environment, nativeEnvironment, objectHandle);

            foreach (var processor in _processors)
            {
                processor.HandleMonitorContendedEnter(environment, nativeEnvironment, thread, @object);
            }
        }

        private void HandleMonitorContendedEntered(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmObjectReference @object = new JvmObjectReference(environment, nativeEnvironment, objectHandle);

            foreach (var processor in _processors)
            {
                processor.HandleMonitorContendedEntered(environment, nativeEnvironment, thread, @object);
            }
        }

        private void HandleResourceExhausted(jvmtiEnvHandle env, JNIEnvHandle jniEnv, JvmResourceExhaustedFlags flags, IntPtr reserved, ModifiedUTF8StringData description)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);

            foreach (var processor in _processors)
            {
                processor.HandleResourceExhausted(environment, nativeEnvironment, flags, reserved, description.GetString());
            }
        }

        private void HandleGarbageCollectionStart(jvmtiEnvHandle env)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleGarbageCollectionStart(environment);
            }
        }

        private void HandleGarbageCollectionFinish(jvmtiEnvHandle env)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleGarbageCollectionFinish(environment);
            }
        }

        private void HandleObjectFree(jvmtiEnvHandle env, long tag)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);

            foreach (var processor in _processors)
            {
                processor.HandleObjectFree(environment, tag);
            }
        }

        private void HandleVMObjectAlloc(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle, jclass objectClassHandle, long size)
        {
            JvmEnvironment environment = JvmEnvironment.GetOrCreateEnvironment(env);
            JvmNativeEnvironment nativeEnvironment = environment.GetNativeFunctionTable(jniEnv);
            JvmThreadReference thread = new JvmThreadReference(environment, nativeEnvironment, threadHandle);
            JvmObjectReference @object = new JvmObjectReference(environment, nativeEnvironment, objectHandle);
            JvmClassReference objectClass = new JvmClassReference(environment, nativeEnvironment, objectClassHandle);

            foreach (var processor in _processors)
            {
                processor.HandleVMObjectAllocation(environment, nativeEnvironment, thread, @object, objectClass, size);
            }
        }
    }
}
