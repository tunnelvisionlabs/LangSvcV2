namespace Tvl.Java.DebugInterface.Client.Request
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugInterface.Request;
    using EventKind = Tvl.Java.DebugInterface.Types.EventKind;
    using RequestId = Tvl.Java.DebugInterface.Types.RequestId;

    internal sealed class EventRequestManager : Mirror, IEventRequestManager
    {
        private readonly List<IAccessWatchpointRequest> _accessWatchpointRequests = new List<IAccessWatchpointRequest>();
        private readonly List<IBreakpointRequest> _breakpointRequests = new List<IBreakpointRequest>();
        private readonly List<IClassPrepareRequest> _classPrepareRequests = new List<IClassPrepareRequest>();
        private readonly List<IClassUnloadRequest> _classUnloadRequests = new List<IClassUnloadRequest>();
        private readonly List<IExceptionRequest> _exceptionRequests = new List<IExceptionRequest>();
        private readonly List<IMethodEntryRequest> _methodEntryRequests = new List<IMethodEntryRequest>();
        private readonly List<IMethodExitRequest> _methodExitRequests = new List<IMethodExitRequest>();
        private readonly List<IModificationWatchpointRequest> _modificationWatchpointRequests = new List<IModificationWatchpointRequest>();
        private readonly List<IMonitorContendedEnterRequest> _monitorContendedEnterRequests = new List<IMonitorContendedEnterRequest>();
        private readonly List<IMonitorContendedEnteredRequest> _monitorContendedEnteredRequests = new List<IMonitorContendedEnteredRequest>();
        private readonly List<IMonitorWaitedRequest> _monitorWaitedRequests = new List<IMonitorWaitedRequest>();
        private readonly List<IMonitorWaitRequest> _monitorWaitRequests = new List<IMonitorWaitRequest>();
        private readonly List<IStepRequest> _stepRequests = new List<IStepRequest>();
        private readonly List<IThreadDeathRequest> _threadDeathRequests = new List<IThreadDeathRequest>();
        private readonly List<IThreadStartRequest> _threadStartRequests = new List<IThreadStartRequest>();
        private readonly List<IVirtualMachineDeathRequest> _virtualMachineDeathRequests = new List<IVirtualMachineDeathRequest>();

        public EventRequestManager(VirtualMachine virtualMachine)
            : base(virtualMachine)
        {
            Contract.Requires(virtualMachine != null);
        }

        public ReadOnlyCollection<IAccessWatchpointRequest> GetAccessWatchpointRequests()
        {
            return _accessWatchpointRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IBreakpointRequest> GetBreakpointRequests()
        {
            return _breakpointRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IClassPrepareRequest> GetClassPrepareRequests()
        {
            return _classPrepareRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IClassUnloadRequest> GetClassUnloadRequests()
        {
            return _classUnloadRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IExceptionRequest> GetExceptionRequests()
        {
            return _exceptionRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IMethodEntryRequest> GetMethodEntryRequests()
        {
            return _methodEntryRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IMethodExitRequest> GetMethodExitRequest()
        {
            return _methodExitRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IModificationWatchpointRequest> GetModificationWatchpointRequests()
        {
            return _modificationWatchpointRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IMonitorContendedEnteredRequest> GetMonitorContendedEnteredRequests()
        {
            return _monitorContendedEnteredRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IMonitorContendedEnterRequest> GetMonitorContendedEnterRequests()
        {
            return _monitorContendedEnterRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IMonitorWaitedRequest> GetMonitorWaitedRequests()
        {
            return _monitorWaitedRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IMonitorWaitRequest> GetMonitorWaitRequests()
        {
            return _monitorWaitRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IStepRequest> GetStepRequests()
        {
            return _stepRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IThreadDeathRequest> GetThreadDeathRequests()
        {
            return _threadDeathRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IThreadStartRequest> GetThreadStartRequests()
        {
            return _threadStartRequests.AsReadOnly();
        }

        public ReadOnlyCollection<IVirtualMachineDeathRequest> GetVirtualMachineDeathRequests()
        {
            return _virtualMachineDeathRequests.AsReadOnly();
        }

        public IAccessWatchpointRequest CreateAccessWatchpointRequest(IField field)
        {
            throw new NotImplementedException();
        }

        public IBreakpointRequest CreateBreakpointRequest(ILocation location)
        {
            throw new NotImplementedException();
        }

        public IClassPrepareRequest CreateClassPrepareRequest()
        {
            throw new NotImplementedException();
        }

        public IClassUnloadRequest CreateClassUnloadRequest()
        {
            throw new NotImplementedException();
        }

        public IExceptionRequest CreateExceptionRequest(IReferenceType referenceType, bool notifyCaught, bool notifyUncaught)
        {
            throw new NotImplementedException();
        }

        public IMethodEntryRequest CreateMethodEntryRequest()
        {
            throw new NotImplementedException();
        }

        public IMethodExitRequest CreateMethodExitRequest()
        {
            throw new NotImplementedException();
        }

        public IModificationWatchpointRequest CreateModificationWatchpointRequest(IField field)
        {
            throw new NotImplementedException();
        }

        public IMonitorContendedEnteredRequest CreateMonitorContendedEnteredRequest()
        {
            throw new NotImplementedException();
        }

        public IMonitorContendedEnterRequest CreateMonitorContendedEnterRequest()
        {
            throw new NotImplementedException();
        }

        public IMonitorWaitedRequest CreateMonitorWaitedRequest()
        {
            throw new NotImplementedException();
        }

        public IMonitorWaitRequest CreateMonitorWaitRequest()
        {
            throw new NotImplementedException();
        }

        public IStepRequest CreateStepRequest(IThreadReference thread, StepSize size, StepDepth depth)
        {
            ThreadReference threadReference = thread as ThreadReference;
            if (threadReference == null && thread != null)
                throw new VirtualMachineMismatchException();

            var request = new StepRequest(VirtualMachine, threadReference, size, depth);
            _stepRequests.Add(request);
            return request;
        }

        public IThreadDeathRequest CreateThreadDeathRequest()
        {
            var request = new ThreadDeathRequest(VirtualMachine);
            _threadDeathRequests.Add(request);
            return request;
        }

        public IThreadStartRequest CreateThreadStartRequest()
        {
            var request = new ThreadStartRequest(VirtualMachine);
            _threadStartRequests.Add(request);
            return request;
        }

        public IVirtualMachineDeathRequest CreateVirtualMachineDeathRequest()
        {
            var request = new VirtualMachineDeathRequest(VirtualMachine);
            _virtualMachineDeathRequests.Add(request);
            return request;
        }

        public void DeleteAllBreakpoints()
        {
            throw new NotImplementedException();
        }

        public void DeleteEventRequest(IEventRequest request)
        {
            throw new NotImplementedException();
        }

        public void DeleteEventRequests(IEnumerable<IEventRequest> requests)
        {
            throw new NotImplementedException();
        }

        internal EventRequest GetEventRequest(EventKind eventKind, RequestId requestId)
        {
            if (requestId.Id == 0)
                return null;

            IEnumerable<EventRequest> requests = null;
            switch (eventKind)
            {
            case EventKind.SingleStep:
                requests = GetStepRequests().Cast<EventRequest>();
                break;

            case EventKind.Breakpoint:
                requests = GetBreakpointRequests().Cast<EventRequest>();
                break;

            case EventKind.Exception:
                requests = GetExceptionRequests().Cast<EventRequest>();
                break;

            case EventKind.ThreadStart:
                requests = GetThreadStartRequests().Cast<EventRequest>();
                break;

            case EventKind.ClassPrepare:
                requests = GetClassPrepareRequests().Cast<EventRequest>();
                break;

            case EventKind.ClassUnload:
                requests = GetClassUnloadRequests().Cast<EventRequest>();
                break;

            case EventKind.FieldAccess:
                requests = GetAccessWatchpointRequests().Cast<EventRequest>();
                break;

            case EventKind.FieldModification:
                requests = GetModificationWatchpointRequests().Cast<EventRequest>();
                break;

            case EventKind.MethodEntry:
                requests = GetMethodEntryRequests().Cast<EventRequest>();
                break;

            case EventKind.MethodExit:
                requests = GetMethodExitRequest().Cast<EventRequest>();
                break;

            case EventKind.VirtualMachineDeath:
                requests = GetVirtualMachineDeathRequests().Cast<EventRequest>();
                break;

            case EventKind.ThreadDeath:
            //case EventKind.ThreadEnd:
                requests = GetThreadDeathRequests().Cast<EventRequest>();
                break;

            case EventKind.FramePop:
            case EventKind.UserDefined:
            case EventKind.ClassLoad:
            case EventKind.ExceptionCatch:
            case EventKind.VirtualMachineDisconnected:
            case EventKind.VirtualMachineStart:
            //case EventKind.VirtualMachineInit:
                // these requests don't have a mirror in this API
                break;

            case EventKind.Invalid:
            default:
                break;
            }

            EventRequest request = null;
            if (requests != null)
                request = requests.FirstOrDefault(i => i.RequestId == requestId);

            return request;
        }
    }
}
