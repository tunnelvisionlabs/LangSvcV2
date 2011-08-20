namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Debugger.Interop;
    using Tvl.VisualStudio.Language.Java.Debugger.Events;
    using Tvl.VisualStudio.Language.Java.Debugger.Extensions;
    using Microsoft.VisualStudio;
    using Debug = System.Diagnostics.Debug;

    partial class JavaDebugProgram
    {
        private class JvmEventsCallback : JvmEventsService.IJvmEventsServiceCallback
        {
            private readonly JavaDebugProgram _program;
            private int _nextThreadId = 1;

            public JvmEventsCallback(JavaDebugProgram program)
            {
                Contract.Requires(program != null);
                _program = program;
            }

            private JavaDebugProgram Program
            {
                get
                {
                    return _program;
                }
            }

            public void Subscribe()
            {
                Program.EventsService.Subscribe(JvmEventsService.JvmEventType.VMInit);
                Program.EventsService.Subscribe(JvmEventsService.JvmEventType.VMStart);
                Program.EventsService.Subscribe(JvmEventsService.JvmEventType.VMDeath);
                Program.EventsService.Subscribe(JvmEventsService.JvmEventType.ThreadStart);
                Program.EventsService.Subscribe(JvmEventsService.JvmEventType.ThreadEnd);
                Program.EventsService.Subscribe(JvmEventsService.JvmEventType.ClassLoad);
                Program.EventsService.Subscribe(JvmEventsService.JvmEventType.ClassPrepare);
            }

            public void OnSingleStep(JvmEventsService.JvmVirtualMachineRemoteHandle virtualMachine, JvmEventsService.JvmThreadRemoteHandle thread, JvmEventsService.JvmRemoteLocation location)
            {
                throw new NotImplementedException();
            }

            public void HandleVMInitialization(JvmEventsService.JvmVirtualMachineRemoteHandle virtualMachine, JvmEventsService.JvmThreadRemoteHandle thread)
            {
            }

            public void HandleVMStart(JvmEventsService.JvmVirtualMachineRemoteHandle virtualMachine)
            {
                IDebugEvent2 @event = new DebugLoadCompleteEvent(enum_EVENTATTRIBUTES.EVENT_SYNC_STOP);
                Guid guid = typeof(IDebugLoadCompleteEvent2).GUID;
                enum_EVENTATTRIBUTES attrib = @event.GetAttributes();
                Program.Callback.Event(Program.DebugEngine, Program.Process, Program, null, @event, ref guid, (uint)attrib);
            }

            public void HandleVMDeath(JvmEventsService.JvmVirtualMachineRemoteHandle virtualMachine)
            {
            }

            public void HandleThreadStart(JvmEventsService.JvmVirtualMachineRemoteHandle virtualMachine, JvmEventsService.JvmThreadRemoteHandle threadHandle)
            {
                int id = _nextThreadId++;

                //JvmToolsService.jvmtiError result = Program.ToolsService.SetTag(virtualMachine, threadHandle, id);
                //Contract.Assert(result == JvmToolsService.jvmtiError.None);

                //long tag;
                //result = Program.ToolsService.GetTag(out tag, virtualMachine, threadHandle);
                //Contract.Assert(result == JvmToolsService.jvmtiError.None);
                //Contract.Assert(tag == id);

                int hashCode;
                JvmToolsService.jvmtiError result = Program.ToolsService.GetObjectHashCode(out hashCode, virtualMachine, threadHandle);

                JavaDebugThread thread = new JavaDebugThread(Program, virtualMachine, threadHandle, id);
                Program._threads[hashCode] = thread;

                IDebugEvent2 @event = new DebugThreadCreateEvent(enum_EVENTATTRIBUTES.EVENT_SYNCHRONOUS);
                Guid guid = typeof(IDebugThreadCreateEvent2).GUID;
                enum_EVENTATTRIBUTES attrib = @event.GetAttributes();
                Program.Callback.Event(Program.DebugEngine, Program.Process, Program, thread, @event, ref guid, (uint)attrib);
            }

            public void HandleThreadEnd(JvmEventsService.JvmVirtualMachineRemoteHandle virtualMachine, JvmEventsService.JvmThreadRemoteHandle threadHandle)
            {
                int hashCode;
                JvmToolsService.jvmtiError result = Program.ToolsService.GetObjectHashCode(out hashCode, virtualMachine, threadHandle);
                if (result == 0)
                {
                    JavaDebugThread thread = Program._threads[hashCode];

                    IDebugEvent2 @event = new DebugThreadDestroyEvent(enum_EVENTATTRIBUTES.EVENT_SYNCHRONOUS, 0);
                    Guid guid = typeof(IDebugThreadDestroyEvent2).GUID;
                    enum_EVENTATTRIBUTES attrib = @event.GetAttributes();
                    Program.Callback.Event(Program.DebugEngine, Program.Process, Program, thread, @event, ref guid, (uint)attrib);

                    Program._threads.Remove(hashCode);
                }
            }

            public void HandleClassLoad(JvmEventsService.JvmVirtualMachineRemoteHandle virtualMachine, JvmEventsService.JvmThreadRemoteHandle threadHandle, JvmEventsService.JvmClassRemoteHandle @class)
            {
                // 'devenv.exe' (Managed (v4.0.30319)): Loaded 'C:\Windows\Microsoft.Net\assembly\GAC_MSIL\Microsoft.VisualStudio.Windows.Forms\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Windows.Forms.dll'
                string programName = Program.GetName();
                string debuggerName;
                Guid debuggerGuid;
                ErrorHandler.ThrowOnFailure(Program.GetEngineInfo(out debuggerName, out debuggerGuid));

                string signature;
                string generic;
                var result = Program.ToolsService.GetClassSignature(out signature, out generic, virtualMachine, @class);

                JavaDebugThread thread = null;
                if (threadHandle.Handle != 0)
                {
                    int hashCode;
                    result = Program.ToolsService.GetObjectHashCode(out hashCode, virtualMachine, threadHandle);
                    Program._threads.TryGetValue(hashCode, out thread);
                }

                string message = string.Format("'{0}' ({1}): Loaded class '{2}'" + Environment.NewLine, programName, debuggerName, signature);
                IDebugEvent2 @event = new DebugOutputStringEvent(enum_EVENTATTRIBUTES.EVENT_SYNCHRONOUS, message);
                Guid guid = typeof(IDebugOutputStringEvent2).GUID;
                enum_EVENTATTRIBUTES attrib = @event.GetAttributes();
                Program.Callback.Event(Program.DebugEngine, Program.Process, Program, thread, @event, ref guid, (uint)attrib);
            }

            public void HandleClassPrepare(JvmEventsService.JvmVirtualMachineRemoteHandle virtualMachine, JvmEventsService.JvmThreadRemoteHandle thread, JvmEventsService.JvmClassRemoteHandle @class)
            {
            }
        }
    }
}
