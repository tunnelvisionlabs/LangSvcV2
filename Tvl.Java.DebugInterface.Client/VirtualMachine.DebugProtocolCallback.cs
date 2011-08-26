namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.Java.DebugInterface.Client.DebugProtocol;
    using Tvl.Java.DebugInterface.Types;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugInterface.Client.Events;
    using SuspendPolicy = Tvl.Java.DebugInterface.Request.SuspendPolicy;
    using Tvl.Java.DebugInterface.Client.Request;
    using System.ServiceModel;

    partial class VirtualMachine
    {
        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, IncludeExceptionDetailInFaults = true)]
        internal class DebugProtocolCallback : IDebugProtocolServiceCallback
        {
            private readonly VirtualMachine _virtualMachine;

            public DebugProtocolCallback(VirtualMachine virtualMachine)
            {
                Contract.Requires(virtualMachine != null);
                _virtualMachine = virtualMachine;
            }

            public VirtualMachine VirtualMachine
            {
                get
                {
                    return _virtualMachine;
                }
            }

            public void VirtualMachineStart(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId threadId)
            {
                ThreadReference thread = VirtualMachine.GetMirrorOf(threadId);
                EventRequest request = VirtualMachine.EventRequestManager.GetEventRequest(EventKind.VirtualMachineStart, requestId);
                ThreadEventArgs e = new ThreadEventArgs(VirtualMachine, (SuspendPolicy)suspendPolicy, request, thread);
                VirtualMachine.EventQueue.OnVirtualMachineStart(e);
            }

            public void SingleStep(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId threadId, Types.Location location)
            {
                ThreadReference thread = VirtualMachine.GetMirrorOf(threadId);
                EventRequest request = VirtualMachine.EventRequestManager.GetEventRequest(EventKind.SingleStep, requestId);
                Location loc = VirtualMachine.GetMirrorOf(location);

                ThreadLocationEventArgs e = new ThreadLocationEventArgs(VirtualMachine, (SuspendPolicy)suspendPolicy, request, thread, loc);
                VirtualMachine.EventQueue.OnSingleStep(e);
            }

            public void Breakpoint(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, Types.Location location)
            {
                throw new NotImplementedException();
            }

            public void MethodEntry(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, Types.Location location)
            {
                throw new NotImplementedException();
            }

            public void MethodExit(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, Types.Location location, Types.Value returnValue)
            {
                throw new NotImplementedException();
            }

            public void MonitorContendedEnter(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, TaggedObjectId @object, Types.Location location)
            {
                throw new NotImplementedException();
            }

            public void MonitorContendedEntered(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, TaggedObjectId @object, Types.Location location)
            {
                throw new NotImplementedException();
            }

            public void MonitorContendedWait(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, TaggedObjectId @object, Types.Location location, TimeSpan timeout)
            {
                throw new NotImplementedException();
            }

            public void MonitorContendedWaited(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, TaggedObjectId @object, Types.Location location, bool timedOut)
            {
                throw new NotImplementedException();
            }

            public void Exception(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, Types.Location location, TaggedObjectId exception, Types.Location catchLocation)
            {
                throw new NotImplementedException();
            }

            public void ThreadStart(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId threadId)
            {
                ThreadReference thread = VirtualMachine.GetMirrorOf(threadId);
                EventRequest request = VirtualMachine.EventRequestManager.GetEventRequest(EventKind.ThreadStart, requestId);
                ThreadEventArgs e = new ThreadEventArgs(VirtualMachine, (SuspendPolicy)suspendPolicy, request, thread);
                VirtualMachine.EventQueue.OnThreadStart(e);
            }

            public void ThreadDeath(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId threadId)
            {
                ThreadReference thread = VirtualMachine.GetMirrorOf(threadId);
                EventRequest request = VirtualMachine.EventRequestManager.GetEventRequest(EventKind.ThreadDeath, requestId);
                ThreadEventArgs e = new ThreadEventArgs(VirtualMachine, (SuspendPolicy)suspendPolicy, request, thread);
                VirtualMachine.EventQueue.OnThreadDeath(e);
            }

            public void ClassPrepare(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, TypeTag typeTag, ReferenceTypeId typeId, string signature, ClassStatus status)
            {
                throw new NotImplementedException();
            }

            public void ClassUnload(Types.SuspendPolicy suspendPolicy, RequestId requestId, string signature)
            {
                throw new NotImplementedException();
            }

            public void FieldAccess(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, Types.Location location, TypeTag typeTag, ReferenceTypeId typeId, FieldId field, TaggedObjectId @object)
            {
                throw new NotImplementedException();
            }

            public void FieldModification(Types.SuspendPolicy suspendPolicy, RequestId requestId, ThreadId thread, Types.Location location, TypeTag typeTag, ReferenceTypeId typeId, FieldId field, TaggedObjectId @object, Types.Value newValue)
            {
                throw new NotImplementedException();
            }

            public void VirtualMachineDeath(Types.SuspendPolicy suspendPolicy, RequestId requestId)
            {
                throw new NotImplementedException();
            }
        }
    }
}
