namespace Tvl.Java.DebugHost.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugInterface.Types;
    using Tvl.Java.DebugHost.Interop;
    using Marshal = System.Runtime.InteropServices.Marshal;
    using System.Windows.Threading;
    using ManualResetEventSlim = System.Threading.ManualResetEventSlim;

    partial class DebugProtocolService
    {
        private class EventProcessor
        {
            private readonly DebugProtocolService _service;
            private readonly jvmtiEventCallbacks _eventCallbacks;

            private ManualResetEventSlim _agentStartedEvent = new ManualResetEventSlim();
            private jthread _agentThread;
            private UnsafeNativeMethods.jvmtiStartFunction _agentCallbackDelegate;
            private Dispatcher _agentEventDispatcher;

            private readonly Dictionary<EventKind, Dictionary<RequestId, EventFilter>> _eventRequests =
                new Dictionary<EventKind, Dictionary<RequestId, EventFilter>>();
            private int _nextRequestId = 1;

            public EventProcessor(DebugProtocolService service)
            {
                Contract.Requires(service != null);
                _service = service;

                _eventCallbacks = new jvmtiEventCallbacks()
                {
                    VMInit = HandleVMInit,
                    VMDeath = HandleVMDeath,
                    ThreadStart = HandleThreadStart,
                    ThreadEnd = HandleThreadEnd,
                    ClassFileLoadHook = HandleClassFileLoadHook,
                    ClassLoad = HandleClassLoad,
                    ClassPrepare = HandleClassPrepare,
                    VMStart = HandleVMStart,
                    Exception = HandleException,
                    ExceptionCatch = HandleExceptionCatch,
                    SingleStep = HandleSingleStep,
                    FramePop = HandleFramePop,
                    Breakpoint = HandleBreakpoint,
                    FieldAccess = HandleFieldAccess,
                    FieldModification = HandleFieldModification,
                    MethodEntry = HandleMethodEntry,
                    MethodExit = HandleMethodExit,
                    NativeMethodBind = HandleNativeMethodBind,
                    CompiledMethodLoad = HandleCompiledMethodLoad,
                    CompiledMethodUnload = HandleCompiledMethodUnload,
                    DynamicCodeGenerated = HandleDynamicCodeGenerated,
                    DataDumpRequest = HandleDataDumpRequest,
                    MonitorWait = HandleMonitorWait,
                    MonitorWaited = HandleMonitorWaited,
                    MonitorContendedEnter = HandleMonitorContendedEnter,
                    MonitorContendedEntered = HandleMonitorContendedEntered,
                    ResourceExhausted = HandleResourceExhausted,
                    GarbageCollectionStart = HandleGarbageCollectionStart,
                    GarbageCollectionFinish = HandleGarbageCollectionFinish,
                    ObjectFree = HandleObjectFree,
                    VMObjectAlloc = HandleVMObjectAlloc
                };
            }

            private IDebugProcotolCallback Callback
            {
                get
                {
                    return _service._callback;
                }
            }

            private JvmtiEnvironment Environment
            {
                get
                {
                    return _service.Environment;
                }
            }

            private jvmtiInterface RawInterface
            {
                get
                {
                    return _service.RawInterface;
                }
            }

            private JavaVM VirtualMachine
            {
                get
                {
                    return _service.VirtualMachine;
                }
            }

            private Dispatcher AgentEventDispatcher
            {
                get
                {
                    return _agentEventDispatcher;
                }
            }

            internal void Attach()
            {
                JvmtiErrorHandler.ThrowOnFailure(Environment.SetEventCallbacks(_eventCallbacks));

                jvmtiCapabilities capabilities;
                JvmtiErrorHandler.ThrowOnFailure(Environment.GetCapabilities(out capabilities));

                JvmtiErrorHandler.ThrowOnFailure(Environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMStart));
                JvmtiErrorHandler.ThrowOnFailure(Environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMInit));
                JvmtiErrorHandler.ThrowOnFailure(Environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.VMDeath));
                JvmtiErrorHandler.ThrowOnFailure(Environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ThreadStart));
                JvmtiErrorHandler.ThrowOnFailure(Environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ThreadEnd));
                JvmtiErrorHandler.ThrowOnFailure(Environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ClassLoad));
                JvmtiErrorHandler.ThrowOnFailure(Environment.SetEventNotificationMode(JvmEventMode.Enable, JvmEventType.ClassPrepare));
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
                _service.Environment.SetEventCallbacks(default(jvmtiEventCallbacks));
            }

            public Error SetEvent(JvmtiEnvironment environment, JniEnvironment nativeEnvironment, EventKind eventKind, SuspendPolicy suspendPolicy, EventRequestModifier[] modifiers, out RequestId requestId)
            {
                requestId = default(RequestId);

                EventRequestModifier locationModifier = default(EventRequestModifier);
                if (eventKind == EventKind.Breakpoint)
                {
                    // we're going to need the location modifier to set the breakpoint
                    locationModifier = modifiers.FirstOrDefault(i => i.Kind == ModifierKind.LocationFilter);
                    if (locationModifier.Kind != ModifierKind.LocationFilter)
                        return Error.IllegalArgument;
                }

                lock (_eventRequests)
                {
                    Dictionary<RequestId, EventFilter> requests;
                    if (!_eventRequests.TryGetValue(eventKind, out requests))
                    {
                        requests = new Dictionary<RequestId, EventFilter>();
                        _eventRequests.Add(eventKind, requests);
                    }

                    requestId = new RequestId(_nextRequestId++);
                    EventFilter filter = EventFilter.CreateFilter(environment, nativeEnvironment, requestId, suspendPolicy, modifiers);
                    requests.Add(requestId, filter);
                    if (requests.Count == 1)
                    {
                        JvmEventType? eventToEnable = GetJvmEventType(eventKind);

                        if (eventToEnable != null)
                        {
                            Environment.SetEventNotificationMode(JvmEventMode.Enable, eventToEnable.Value);
                        }
                    }
                }

                if (eventKind == EventKind.Breakpoint)
                {
                    Contract.Assert(locationModifier.Kind == ModifierKind.LocationFilter);
                    jmethodID methodId = locationModifier.Location.Method;
                    jlocation location = new jlocation((long)locationModifier.Location.Index);
                    jvmtiError error = Environment.SetBreakpoint(methodId, location);
                    if (error != jvmtiError.None)
                        return GetStandardError(error);
                }

                return Error.None;
            }

            public Error ClearEvent(EventKind eventKind, RequestId requestId)
            {
                lock (_eventRequests)
                {
                    Dictionary<RequestId, EventFilter> requests;
                    if (!_eventRequests.TryGetValue(eventKind, out requests))
                        return Error.None;

                    EventFilter eventFilter;
                    if (!requests.TryGetValue(requestId, out eventFilter))
                        return Error.None;

                    requests.Remove(requestId);
                    if (requests.Count == 0)
                    {
                        JvmEventType? eventToDisable = GetJvmEventType(eventKind);
                        if (eventToDisable != null)
                        {
                            Environment.SetEventNotificationMode(JvmEventMode.Disable, eventToDisable.Value);
                        }
                    }

                    if (eventKind == EventKind.Breakpoint)
                    {
                        LocationEventFilter locationFilter = eventFilter as LocationEventFilter;
                        if (locationFilter == null)
                        {
                            AggregateEventFilter aggregateFilter = eventFilter as AggregateEventFilter;
                            Contract.Assert(aggregateFilter != null);
                            locationFilter = aggregateFilter.Filters.OfType<LocationEventFilter>().FirstOrDefault();
                        }

                        Contract.Assert(locationFilter != null);
                        jmethodID methodId = locationFilter.Location.Method;
                        jlocation location = new jlocation((long)locationFilter.Location.Index);
                        jvmtiError error = Environment.ClearBreakpoint(methodId, location);
                        if (error != jvmtiError.None)
                            return GetStandardError(error);
                    }

                    return Error.None;
                }
            }

            private static JvmEventType? GetJvmEventType(EventKind eventKind)
            {
                switch (eventKind)
                {
                case EventKind.Breakpoint:
                    return JvmEventType.Breakpoint;

                case EventKind.SingleStep:
                    return JvmEventType.SingleStep;

                case EventKind.ThreadStart:
                    return JvmEventType.ThreadStart;

                case EventKind.ThreadEnd:
                    return JvmEventType.ThreadEnd;

                case EventKind.ClassLoad:
                    return JvmEventType.ClassLoad;

                case EventKind.ClassPrepare:
                    return JvmEventType.ClassPrepare;

                case EventKind.ClassUnload:
                    throw new NotSupportedException();

                case EventKind.Exception:
                    return JvmEventType.Exception;

                case EventKind.ExceptionCatch:
                    return JvmEventType.ExceptionCatch;

                case EventKind.FieldAccess:
                    return JvmEventType.FieldAccess;

                case EventKind.FieldModification:
                    return JvmEventType.FieldModification;

                case EventKind.FramePop:
                    return JvmEventType.FramePop;

                case EventKind.MethodEntry:
                    return JvmEventType.MethodEntry;

                case EventKind.MethodExit:
                    return JvmEventType.MethodExit;

                default:
                    return null;
                }
            }

            public Error ClearAllBreakpoints()
            {
                throw new NotImplementedException();
            }

            private EventFilter[] GetEventFilters(EventKind eventKind)
            {
                lock (_eventRequests)
                {
                    Dictionary<RequestId, EventFilter> requests;
                    if (!_eventRequests.TryGetValue(eventKind, out requests))
                        return new EventFilter[0];

                    return requests.Values.ToArray();
                }
            }

            private jvmtiError ApplySuspendPolicy(JvmtiEnvironment environment, JniEnvironment nativeEnvironment, SuspendPolicy policy, ThreadId eventThread)
            {
                if (policy == SuspendPolicy.EventThread && eventThread == default(ThreadId))
                {
                    return jvmtiError.InvalidThread;
                }

                switch (policy)
                {
                case SuspendPolicy.None:
                    return jvmtiError.None;

                case SuspendPolicy.EventThread:
                    return environment.SuspendThread(nativeEnvironment, eventThread);

                case SuspendPolicy.All:
                    ThreadId[] requestList;
                    JvmtiErrorHandler.ThrowOnFailure(environment.GetAllThreads(nativeEnvironment, out requestList));
                    jvmtiError[] errors;
                    return environment.SuspendThreads(nativeEnvironment, requestList, out errors);

                default:
                    throw new ArgumentException("Invalid suspend policy.");
                }
            }

            private void HandleVMInit(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
            {
                JvmtiEnvironment environment = JvmtiEnvironment.GetOrCreateInstance(_service.VirtualMachine, env);
                JniEnvironment nativeEnvironment;

                if (AgentEventDispatcher == null)
                {
                    if (VirtualMachine.IsAgentThread.Value)
                        throw new InvalidOperationException();

                    nativeEnvironment = JniEnvironment.GetOrCreateInstance(jniEnv);
                    InitializeAgentThread(environment, nativeEnvironment);
                }

                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread> method = HandleVMInit;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle);
                    return;
                }

                JniErrorHandler.ThrowOnFailure(VirtualMachine.AttachCurrentThreadAsDaemon(environment, out nativeEnvironment, true));
                ThreadId threadId = VirtualMachine.TrackLocalThreadReference(threadHandle, environment, nativeEnvironment, true);

                bool sent = false;
                EventFilter[] filters = GetEventFilters(EventKind.VirtualMachineStart);
                foreach (var filter in filters)
                {
                    if (filter.ProcessEvent(environment, nativeEnvironment, threadId, default(TaggedReferenceTypeId), default(Location?)))
                    {
                        ApplySuspendPolicy(environment, nativeEnvironment, filter.SuspendPolicy, threadId);
                        Callback.VirtualMachineStart(filter.SuspendPolicy, filter.RequestId, threadId);
                        sent = true;
                    }
                }

                if (!sent)
                {
                    ApplySuspendPolicy(environment, nativeEnvironment, SuspendPolicy.All, threadId);
                    Callback.VirtualMachineStart(SuspendPolicy.All, default(RequestId), threadId);
                }

            }

            private void InitializeAgentThread(JvmtiEnvironment environment, JniEnvironment nativeEnvironment)
            {
                _agentStartedEvent = new ManualResetEventSlim(false);
                _agentThread = CreateAgentThread(nativeEnvironment);
                _agentCallbackDelegate = AgentDispatcherThread;
                JvmtiErrorHandler.ThrowOnFailure(environment.RawInterface.RunAgentThread(environment, _agentThread, _agentCallbackDelegate, IntPtr.Zero, JvmThreadPriority.Maximum));
                _agentStartedEvent.Wait();
            }

            private jthread CreateAgentThread(JniEnvironment nativeEnvironment)
            {
                jclass @class = nativeEnvironment.FindClass("java/lang/Thread");
                if (@class == jclass.Null)
                    throw new Exception("ERROR: JNI: Cannot find java/lang/Thread with FindClass.");

                nativeEnvironment.ExceptionClear();

                jmethodID method = nativeEnvironment.GetMethodId(@class, "<init>", "()V");
                if (method == jmethodID.Null)
                    throw new Exception("Cannot find Thread constructor method.");

                nativeEnvironment.ExceptionClear();

                jthread result = (jthread)nativeEnvironment.NewObject(@class, method);
                if (result == jthread.Null)
                    throw new Exception("Cannot create new Thread object");

                nativeEnvironment.ExceptionClear();

                jthread agentThread = (jthread)nativeEnvironment.NewGlobalReference(result);
                if (result == jthread.Null)
                    throw new Exception("Cannot create a new global reference for the agent thread.");

                nativeEnvironment.ExceptionClear();
                // don't need to keep the local reference around
                nativeEnvironment.DeleteLocalReference(result);
                nativeEnvironment.ExceptionClear();

                return agentThread;
            }

            private void AgentDispatcherThread(jvmtiEnvHandle env, JNIEnvHandle jniEnv, IntPtr arg)
            {
                JniEnvironment nativeEnvironment;
                VirtualMachine.AttachCurrentThreadAsDaemon(Environment, out nativeEnvironment, true);
                _agentEventDispatcher = Dispatcher.CurrentDispatcher;
                _agentStartedEvent.Set();
                Dispatcher.Run();
            }

            private void HandleVMDeath(jvmtiEnvHandle env, JNIEnvHandle jniEnv)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle> method = HandleVMDeath;
                    AgentEventDispatcher.Invoke(method, env, jniEnv);
                    return;
                }

                JvmtiEnvironment environment = JvmtiEnvironment.GetOrCreateInstance(_service.VirtualMachine, env);

                JniEnvironment nativeEnvironment;
                JniErrorHandler.ThrowOnFailure(VirtualMachine.AttachCurrentThreadAsDaemon(environment, out nativeEnvironment, true));

                EventFilter[] filters = GetEventFilters(EventKind.VirtualMachineDeath);
                foreach (var filter in filters)
                {
                    if (filter.ProcessEvent(environment, nativeEnvironment, default(ThreadId), default(TaggedReferenceTypeId), default(Location?)))
                    {
                        ApplySuspendPolicy(environment, nativeEnvironment, filter.SuspendPolicy, default(ThreadId));
                        Callback.VirtualMachineDeath(filter.SuspendPolicy, filter.RequestId);
                    }
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleVMDeath(environment);
                //}

                //environment.VirtualMachine.ShutdownAgentDispatchers();
            }

            private void HandleThreadStart(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
            {
                /* This event is always sent on the thread that's starting. if it's an agent thread, just
                 * ignore the event to hide it from the IDE.
                 */
                if (VirtualMachine.IsAgentThread.Value)
                    return;

                // ignore events before VMInit
                if (AgentEventDispatcher == null)
                    return;

                // dispatch this call to an agent thread
                Action<jvmtiEnvHandle, JNIEnvHandle, jthread> method = HandleThreadStartImpl;
                AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle);
                return;
            }

            private void HandleThreadStartImpl(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
            {
                JvmtiEnvironment environment = JvmtiEnvironment.GetOrCreateInstance(_service.VirtualMachine, env);
                JniEnvironment nativeEnvironment;
                JniErrorHandler.ThrowOnFailure(VirtualMachine.AttachCurrentThreadAsDaemon(environment, out nativeEnvironment, true));

                ThreadId threadId = VirtualMachine.TrackLocalThreadReference(threadHandle, environment, nativeEnvironment, true);
                EventFilter[] filters = GetEventFilters(EventKind.ThreadStart);
                foreach (var filter in filters)
                {
                    if (filter.ProcessEvent(environment, nativeEnvironment, threadId, default(TaggedReferenceTypeId), default(Location?)))
                    {
                        ApplySuspendPolicy(environment, nativeEnvironment, filter.SuspendPolicy, threadId);
                        Callback.ThreadStart(filter.SuspendPolicy, filter.RequestId, threadId);
                    }
                }
            }

            private void HandleThreadEnd(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
            {
                /* This event is always sent on the thread that's starting. if it's an agent thread, just
                 * ignore the event to hide it from the IDE.
                 */
                if (VirtualMachine.IsAgentThread.Value)
                    return;

                // ignore events before VMInit
                if (AgentEventDispatcher == null)
                    return;

                // dispatch this call to an agent thread
                Action<jvmtiEnvHandle, JNIEnvHandle, jthread> method = HandleThreadEndImpl;
                AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle);
                return;
            }

            private void HandleThreadEndImpl(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle)
            {
                JvmtiEnvironment environment = JvmtiEnvironment.GetOrCreateInstance(_service.VirtualMachine, env);
                JniEnvironment nativeEnvironment;
                JniErrorHandler.ThrowOnFailure(VirtualMachine.AttachCurrentThreadAsDaemon(environment, out nativeEnvironment, true));
                ThreadId threadId = VirtualMachine.TrackLocalThreadReference(threadHandle, environment, nativeEnvironment, false);
                EventFilter[] filters = GetEventFilters(EventKind.ThreadEnd);
                foreach (var filter in filters)
                {
                    if (filter.ProcessEvent(environment, nativeEnvironment, threadId, default(TaggedReferenceTypeId), default(Location?)))
                    {
                        ApplySuspendPolicy(environment, nativeEnvironment, filter.SuspendPolicy, threadId);
                        Callback.ThreadDeath(filter.SuspendPolicy, filter.RequestId, threadId);
                    }
                }
            }

            private void HandleClassFileLoadHook(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jclass classBeingRedefinedHandle, jobject loaderHandle, ModifiedUTF8StringData name, jobject protectionDomainHandle, int classDataLength, IntPtr classData, ref int newClassDataLength, ref IntPtr newClassData)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    //Action<jvmtiEnvHandle, JNIEnvHandle, jclass, jobject, ModifiedUTF8StringData, jobject, int, IntPtr> method = HandleClassFileLoadHook;
                    //AgentEventDispatcher.Invoke(method, env, jniEnv);
                    //return;
                    throw new NotImplementedException();
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmClassReference classBeingRedefined = JvmClassReference.FromHandle(environment, jniEnv, classBeingRedefinedHandle, true);
                //JvmObjectReference loader = JvmObjectReference.FromHandle(environment, jniEnv, loaderHandle, true);
                //JvmObjectReference protectionDomain = JvmObjectReference.FromHandle(environment, jniEnv, protectionDomainHandle, true);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleClassFileLoadHook(environment, classBeingRedefined, loader, name.GetString(), protectionDomain);
                //}
            }

            private void HandleClassLoad(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jclass classHandle)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jclass> method = HandleClassLoad;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, classHandle);
                    return;
                }

#if false
                ThreadId threadId = GetObjectId(ref threadHandle);
                ClassId classId = GetObjectId(ref classHandle);
                EventFilter[] filters = GetEventFilters(EventKind.ClassLoad);
                foreach (var filter in filters)
                {
                    if (filter.ProcessEvent(threadId, classId))
                    {
                        ApplySuspendPolicy(filter.SuspendPolicy, threadId);
                        Callback.ClassLoad(filter.SuspendPolicy, filter.RequestId, threadId);
                    }
                }
#endif
            }

            private void HandleClassPrepare(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jclass classHandle)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jclass> method = HandleClassPrepare;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, classHandle);
                    return;
                }

                JvmtiEnvironment environment = JvmtiEnvironment.GetOrCreateInstance(_service.VirtualMachine, env);
                JniEnvironment nativeEnvironment;
                JniErrorHandler.ThrowOnFailure(VirtualMachine.AttachCurrentThreadAsDaemon(environment, out nativeEnvironment, true));

                string signature;
                IntPtr signaturePtr;
                IntPtr genericPtr;
                JvmtiErrorHandler.ThrowOnFailure(RawInterface.GetClassSignature(env, classHandle, out signaturePtr, out genericPtr));
                try
                {
                    unsafe
                    {
                        signature = ModifiedUTF8Encoding.GetString((byte*)signaturePtr);
                    }
                }
                finally
                {
                    RawInterface.Deallocate(env, signaturePtr);
                    RawInterface.Deallocate(env, genericPtr);
                }

                ClassStatus classStatus = 0;
                jvmtiClassStatus internalClassStatus;
                JvmtiErrorHandler.ThrowOnFailure(RawInterface.GetClassStatus(env, classHandle, out internalClassStatus));
                if ((internalClassStatus & jvmtiClassStatus.Error) != 0)
                    classStatus |= ClassStatus.Error;
                if ((internalClassStatus & jvmtiClassStatus.Initialized) != 0)
                    classStatus |= ClassStatus.Initialized;
                if ((internalClassStatus & jvmtiClassStatus.Prepared) != 0)
                    classStatus |= ClassStatus.Prepared;
                if ((internalClassStatus & jvmtiClassStatus.Verified) != 0)
                    classStatus |= ClassStatus.Verified;

                ThreadId threadId = VirtualMachine.TrackLocalThreadReference(threadHandle, environment, nativeEnvironment, true);
                TaggedReferenceTypeId classId = VirtualMachine.TrackLocalClassReference(classHandle, environment, nativeEnvironment, true);
                EventFilter[] filters = GetEventFilters(EventKind.ClassPrepare);
                foreach (var filter in filters)
                {
                    if (filter.ProcessEvent(environment, nativeEnvironment, threadId, classId, default(Location?)))
                    {
                        ApplySuspendPolicy(environment, nativeEnvironment, filter.SuspendPolicy, threadId);
                        Callback.ClassPrepare(filter.SuspendPolicy, filter.RequestId, threadId, classId.TypeTag, classId.TypeId, signature, classStatus);
                    }
                }
            }

            private void HandleVMStart(jvmtiEnvHandle env, JNIEnvHandle jniEnv)
            {
                /**********************
                 * Note: there are no dispatchers available at this point.
                 */

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleVMStart(environment);
                //}
            }

            private void HandleException(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, jlocation jlocation, jobject exceptionHandle, jmethodID catchMethodId, jlocation catchjLocation)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID, jlocation, jobject, jmethodID, jlocation> method = HandleException;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, methodId, jlocation, exceptionHandle, catchMethodId, catchjLocation);
                    return;
                }

                JvmtiEnvironment environment = JvmtiEnvironment.GetOrCreateInstance(_service.VirtualMachine, env);

                //Location location = new Location();
                //Location catchLocation = new Location();
                throw new NotImplementedException();

#if false
                ThreadId threadId = GetObjectId(ref threadHandle);
                TaggedObjectId exception = GetObjectId(ref exceptionHandle);
                EventFilter[] filters = GetEventFilters(EventKind.Exception);
                foreach (var filter in filters)
                {
                    if (filter.ProcessEvent(threadId, default(TaggedReferenceTypeId)))
                    {
                        ApplySuspendPolicy(environment, filter.SuspendPolicy, threadId);
                        Callback.HandleException(filter.SuspendPolicy, filter.RequestId, threadId, location, exception, catchLocation);
                    }
                }
#endif

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmLocation location = new JvmLocation(environment, method, jlocation);
                //JvmObjectReference exception = JvmObjectReference.FromHandle(environment, jniEnv, exceptionHandle, true);
                //JvmLocation catchLocation = new JvmLocation(environment, catchMethod, catchjLocation);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleException(environment, thread, location, exception, catchLocation);
                //}
            }

            private void HandleExceptionCatch(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, jlocation jlocation, jobject exceptionHandle)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID, jlocation, jobject> method = HandleExceptionCatch;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, methodId, jlocation, exceptionHandle);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmLocation location = new JvmLocation(environment, method, jlocation);
                //JvmObjectReference exception = JvmObjectReference.FromHandle(environment, jniEnv, exceptionHandle, true);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleExceptionCatch(environment, thread, location, exception);
                //}
            }

            private void HandleSingleStep(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, jlocation jlocation)
            {
                //if (!VirtualMachine.IsAgentThread.Value)
                //{
                //    // ignore events before VMInit
                //    if (AgentEventDispatcher == null)
                //        return;

                //    // dispatch this call to an agent thread
                //    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID, jlocation> invokeMethod = HandleSingleStep;
                //    AgentEventDispatcher.Invoke(invokeMethod, env, jniEnv, threadHandle, methodId, jlocation);
                //    return;
                //}

                JvmtiEnvironment environment = JvmtiEnvironment.GetOrCreateInstance(_service.VirtualMachine, env);
                JniEnvironment nativeEnvironment = JniEnvironment.GetOrCreateInstance(jniEnv);

                ThreadId threadId = VirtualMachine.TrackLocalThreadReference(threadHandle, environment, nativeEnvironment, true);

                TaggedReferenceTypeId declaringClass;
                MethodId method = new MethodId(methodId.Handle);
                ulong index = (ulong)jlocation.Value;
                JvmtiErrorHandler.ThrowOnFailure(environment.GetMethodDeclaringClass(nativeEnvironment, method, out declaringClass));
                Location location = new Location(declaringClass, method, index);

                EventFilter[] filters = GetEventFilters(EventKind.SingleStep);
                foreach (var filter in filters)
                {
                    if (filter.ProcessEvent(environment, nativeEnvironment, threadId, default(TaggedReferenceTypeId), location))
                    {
                        SendSingleStepEvent(environment, filter, threadId, location);
                    }
                }
            }

            private void SendSingleStepEvent(JvmtiEnvironment environment, EventFilter filter, ThreadId threadId, Location location)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<JvmtiEnvironment, EventFilter, ThreadId, Location> invokeMethod = SendSingleStepEvent;
                    AgentEventDispatcher.Invoke(invokeMethod, environment, filter, threadId, location);
                    return;
                }

                JniEnvironment nativeEnvironment;
                JniErrorHandler.ThrowOnFailure(VirtualMachine.AttachCurrentThreadAsDaemon(environment, out nativeEnvironment, true));

                ApplySuspendPolicy(environment, nativeEnvironment, filter.SuspendPolicy, threadId);
                Callback.SingleStep(filter.SuspendPolicy, filter.RequestId, threadId, location);
            }

            private void HandleFramePop(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, bool wasPoppedByException)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID, bool> method = HandleFramePop;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, methodId, wasPoppedByException);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmMethod method = new JvmMethod(environment, methodId);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleFramePop(environment, thread, method, wasPoppedByException);
                //}
            }

            private void HandleBreakpoint(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, jlocation jlocation)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID, jlocation> invokeMethod = HandleBreakpoint;
                    AgentEventDispatcher.Invoke(invokeMethod, env, jniEnv, threadHandle, methodId, jlocation);
                    return;
                }

                JvmtiEnvironment environment = JvmtiEnvironment.GetOrCreateInstance(_service.VirtualMachine, env);
                JniEnvironment nativeEnvironment;
                JniErrorHandler.ThrowOnFailure(VirtualMachine.AttachCurrentThreadAsDaemon(environment, out nativeEnvironment, true));

                ThreadId threadId = VirtualMachine.TrackLocalThreadReference(threadHandle, environment, nativeEnvironment, true);

                TaggedReferenceTypeId declaringClass;
                MethodId method = new MethodId(methodId.Handle);
                ulong index = (ulong)jlocation.Value;
                JvmtiErrorHandler.ThrowOnFailure(environment.GetMethodDeclaringClass(nativeEnvironment, method, out declaringClass));
                Location location = new Location(declaringClass, method, index);

                EventFilter[] filters = GetEventFilters(EventKind.Breakpoint);
                foreach (var filter in filters)
                {
                    if (filter.ProcessEvent(environment, nativeEnvironment, threadId, default(TaggedReferenceTypeId), location))
                    {
                        ApplySuspendPolicy(environment, nativeEnvironment, filter.SuspendPolicy, threadId);
                        Callback.Breakpoint(filter.SuspendPolicy, filter.RequestId, threadId, location);
                    }
                }
            }

            private void HandleFieldAccess(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, jlocation jlocation, jclass fieldClassHandle, jobject objectHandle, jfieldID fieldId)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID, jlocation, jclass, jobject, jfieldID> method = HandleFieldAccess;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, methodId, jlocation, fieldClassHandle, objectHandle, fieldId);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmLocation location = new JvmLocation(environment, method, jlocation);
                //JvmClassReference fieldClass = JvmClassReference.FromHandle(environment, jniEnv, fieldClassHandle, true);
                //JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);
                //JvmField field = new JvmField(environment, fieldId);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleFieldAccess(environment, thread, location, fieldClass, @object, field);
                //}
            }

            private void HandleFieldModification(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, jlocation jlocation, jclass fieldClassHandle, jobject objectHandle, jfieldID fieldId, byte signatureType, jvalue newValue)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID, jlocation, jclass, jobject, jfieldID, byte, jvalue> method = HandleFieldModification;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, methodId, jlocation, fieldClassHandle, objectHandle, fieldId, signatureType, newValue);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmLocation location = new JvmLocation(environment, method, jlocation);
                //JvmClassReference fieldClass = JvmClassReference.FromHandle(environment, jniEnv, fieldClassHandle, true);
                //JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);
                //JvmField field = new JvmField(environment, fieldId);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleFieldModification(environment, thread, location, fieldClass, @object, field, signatureType, newValue);
                //}
            }

            private void HandleMethodEntry(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID> method = HandleMethodEntry;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, methodId);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmMethod method = new JvmMethod(environment, methodId);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleMethodEntry(environment, thread, method);
                //}
            }

            private void HandleMethodExit(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, bool wasPoppedByException, jvalue returnValue)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID, bool, jvalue> method = HandleMethodExit;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, methodId, wasPoppedByException, returnValue);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmMethod method = new JvmMethod(environment, methodId);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleMethodExit(environment, thread, method, wasPoppedByException, returnValue);
                //}
            }

            private void HandleNativeMethodBind(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jmethodID methodId, IntPtr address, ref IntPtr newAddressPtr)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    //Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jmethodID, IntPtr, ReferenceTypeData intptr> method = HandleNativeMethodBind;
                    //AgentEventDispatcher.Invoke(method, env, jniEnv);
                    //return;
                    throw new NotImplementedException();
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmMethod method = new JvmMethod(environment, methodId);

                //foreach (var processor in _processors)
                //{
                //    IntPtr? newAddress = null;
                //    processor.HandleNativeMethodBind(environment, thread, method, address, ref newAddress);
                //}
            }

            private void HandleCompiledMethodLoad(jvmtiEnvHandle env, jmethodID methodId, int codeSize, IntPtr codeAddress, int mapLength, jvmtiAddressLocationMap[] map, IntPtr compileInfo)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, jmethodID, int, IntPtr, int, jvmtiAddressLocationMap[], IntPtr> method = HandleCompiledMethodLoad;
                    AgentEventDispatcher.Invoke(method, env, methodId, codeSize, codeAddress, mapLength, map, compileInfo);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

                //foreach (var processor in _processors)
                //{
                //    throw new NotImplementedException();
                //    //processor.HandleCompiledMethodLoad(environment, method, codeSize, codeAddress, map2, compileInfo);
                //}
            }

            private void HandleCompiledMethodUnload(jvmtiEnvHandle env, jmethodID methodId, IntPtr codeAddress)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, jmethodID, IntPtr> method = HandleCompiledMethodUnload;
                    AgentEventDispatcher.Invoke(method, env, methodId, codeAddress);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmMethod method = new JvmMethod(environment, methodId);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleCompiledMethodUnload(environment, method, codeAddress);
                //}
            }

            private void HandleDynamicCodeGenerated(jvmtiEnvHandle env, ModifiedUTF8StringData name, IntPtr address, int length)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, ModifiedUTF8StringData, IntPtr, int> method = HandleDynamicCodeGenerated;
                    AgentEventDispatcher.Invoke(method, env, name, address, length);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleDynamicCodeGenerated(environment, name.GetString(), address, length);
                //}
            }

            private void HandleDataDumpRequest(jvmtiEnvHandle env)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle> method = HandleDataDumpRequest;
                    AgentEventDispatcher.Invoke(method, env);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleDataDumpRequest(environment);
                //}
            }

            private void HandleMonitorWait(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle, long millisecondsTimeout)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jobject, long> method = HandleMonitorWait;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, objectHandle, millisecondsTimeout);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleMonitorWait(environment, thread, @object, millisecondsTimeout);
                //}
            }

            private void HandleMonitorWaited(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle, bool timedOut)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jobject, bool> method = HandleMonitorWaited;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, objectHandle, timedOut);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleMonitorWaited(environment, thread, @object, timedOut);
                //}
            }

            private void HandleMonitorContendedEnter(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jobject> method = HandleMonitorContendedEnter;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, objectHandle);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleMonitorContendedEnter(environment, thread, @object);
                //}
            }

            private void HandleMonitorContendedEntered(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jobject> method = HandleMonitorContendedEntered;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, objectHandle);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleMonitorContendedEntered(environment, thread, @object);
                //}
            }

            private void HandleResourceExhausted(jvmtiEnvHandle env, JNIEnvHandle jniEnv, JvmResourceExhaustedFlags flags, IntPtr reserved, ModifiedUTF8StringData description)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, JvmResourceExhaustedFlags, IntPtr, ModifiedUTF8StringData> method = HandleResourceExhausted;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, flags, reserved, description);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleResourceExhausted(environment, flags, reserved, description.GetString());
                //}
            }

            private void HandleGarbageCollectionStart(jvmtiEnvHandle env)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle> method = HandleGarbageCollectionStart;
                    AgentEventDispatcher.Invoke(method, env);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleGarbageCollectionStart(environment);
                //}
            }

            private void HandleGarbageCollectionFinish(jvmtiEnvHandle env)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle> method = HandleGarbageCollectionFinish;
                    AgentEventDispatcher.Invoke(method, env);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleGarbageCollectionFinish(environment);
                //}
            }

            private void HandleObjectFree(jvmtiEnvHandle env, long tag)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, long> method = HandleObjectFree;
                    AgentEventDispatcher.Invoke(method, env, tag);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleObjectFree(environment, tag);
                //}
            }

            private void HandleVMObjectAlloc(jvmtiEnvHandle env, JNIEnvHandle jniEnv, jthread threadHandle, jobject objectHandle, jclass objectClassHandle, long size)
            {
                if (!VirtualMachine.IsAgentThread.Value)
                {
                    // ignore events before VMInit
                    if (AgentEventDispatcher == null)
                        return;

                    // dispatch this call to an agent thread
                    Action<jvmtiEnvHandle, JNIEnvHandle, jthread, jobject, jclass, long> method = HandleVMObjectAlloc;
                    AgentEventDispatcher.Invoke(method, env, jniEnv, threadHandle, objectHandle, objectClassHandle, size);
                    return;
                }

                //JvmEnvironment environment = JvmEnvironment.GetEnvironment(env);
                //JvmThreadReference thread = JvmThreadReference.FromHandle(environment, jniEnv, threadHandle, true);
                //JvmObjectReference @object = JvmObjectReference.FromHandle(environment, jniEnv, objectHandle, true);
                //JvmClassReference objectClass = JvmClassReference.FromHandle(environment, jniEnv, objectClassHandle, true);

                //foreach (var processor in _processors)
                //{
                //    processor.HandleVMObjectAllocation(environment, thread, @object, objectClass, size);
                //}
            }
        }
    }
}
