namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;
    using jvmtiThreadState = Tvl.VisualStudio.Language.Java.JvmToolsService.jvmtiThreadState;

    [ComVisible(true)]
    public class JavaDebugThread
        : IDebugThread3
        , IDebugThread2
        , IDebugQueryEngine2
    {
        private readonly JavaDebugProgram _program;
        private readonly JvmEventsService.JvmVirtualMachineRemoteHandle _virtualMachine;
        private readonly JvmEventsService.JvmThreadRemoteHandle _threadHandle;
        private readonly int _threadId;

        private int _suspendCount;

        public JavaDebugThread(JavaDebugProgram program, JvmEventsService.JvmVirtualMachineRemoteHandle virtualMachine, JvmEventsService.JvmThreadRemoteHandle threadHandle, int threadId)
        {
            Contract.Requires<ArgumentNullException>(program != null, "program");

            _program = program;
            _virtualMachine = virtualMachine;
            _threadHandle = threadHandle;
            _threadId = threadId;
        }

        #region IDebugThread2 Members

        public int CanSetNextStatement(IDebugStackFrame2 pStackFrame, IDebugCodeContext2 pCodeContext)
        {
            throw new NotImplementedException();
        }

        public int EnumFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, out IEnumDebugFrameInfo2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int GetLogicalThread(IDebugStackFrame2 pStackFrame, out IDebugLogicalThread2 ppLogicalThread)
        {
            throw new NotImplementedException();
        }

        public int GetName(out string pbstrName)
        {
            JvmToolsService.JvmThreadRemoteInfo info;
            JvmToolsService.jvmtiError result = _program.ToolsService.GetThreadInfo(out info, _virtualMachine, _threadHandle);
            pbstrName = info.Name;

            return result == JvmToolsService.jvmtiError.None ? VSConstants.S_OK : VSConstants.E_FAIL;
        }

        public int GetProgram(out IDebugProgram2 ppProgram)
        {
            ppProgram = _program;
            return VSConstants.S_OK;
        }

        public int GetThreadId(out uint pdwThreadId)
        {
            pdwThreadId = (uint)_threadId;
            return VSConstants.S_OK;
        }

        public int GetThreadProperties(enum_THREADPROPERTY_FIELDS dwFields, THREADPROPERTIES[] ptp)
        {
            if (ptp == null)
                throw new ArgumentNullException("ptp");
            if (ptp.Length == 0)
                throw new ArgumentException();

            ptp[0].dwFields = 0;

            if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_ID) != 0)
            {
                ptp[0].dwFields |= enum_THREADPROPERTY_FIELDS.TPF_ID;
                ptp[0].dwThreadId = (uint)_threadId;
            }

            if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_LOCATION) != 0)
            {
                ptp[0].dwFields |= enum_THREADPROPERTY_FIELDS.TPF_LOCATION;
                ptp[0].bstrLocation = "Unknown";
            }

            JvmToolsService.JvmThreadRemoteInfo info;
            JvmToolsService.jvmtiError result = _program.ToolsService.GetThreadInfo(out info, _virtualMachine, _threadHandle);

            if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_NAME) != 0)
            {
                ptp[0].dwFields |= enum_THREADPROPERTY_FIELDS.TPF_NAME;
                ptp[0].bstrName = info.Name;
            }

            if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_PRIORITY) != 0)
            {
                ptp[0].dwFields |= enum_THREADPROPERTY_FIELDS.TPF_PRIORITY;
                ptp[0].bstrPriority = info.Priority.ToString();
            }

            if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_STATE) != 0)
            {
                ptp[0].dwFields |= enum_THREADPROPERTY_FIELDS.TPF_STATE;

                enum_THREADSTATE vsthreadState = 0;
                jvmtiThreadState threadState;
                result = _program.ToolsService.GetThreadState(out threadState, _virtualMachine, _threadHandle);

                if ((threadState & jvmtiThreadState.Suspended) != 0)
                    vsthreadState = enum_THREADSTATE.THREADSTATE_FROZEN;
                else if ((threadState & jvmtiThreadState.Terminated) != 0)
                    vsthreadState = enum_THREADSTATE.THREADSTATE_DEAD;
                else if ((threadState & jvmtiThreadState.Interrupted) != 0)
                    vsthreadState = enum_THREADSTATE.THREADSTATE_STOPPED;
                else
                    vsthreadState = enum_THREADSTATE.THREADSTATE_RUNNING;

                ptp[0].dwThreadState = (uint)vsthreadState;
            }

            if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_SUSPENDCOUNT) != 0)
            {
                ptp[0].dwFields |= enum_THREADPROPERTY_FIELDS.TPF_SUSPENDCOUNT;
                ptp[0].dwSuspendCount = (uint)_suspendCount;
            }

            return VSConstants.S_OK;
        }

        public int Resume(out uint pdwSuspendCount)
        {
            throw new NotImplementedException();
        }

        public int SetNextStatement(IDebugStackFrame2 pStackFrame, IDebugCodeContext2 pCodeContext)
        {
            throw new NotImplementedException();
        }

        public int SetThreadName(string pszName)
        {
            throw new NotImplementedException();
        }

        public int Suspend(out uint pdwSuspendCount)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugThread3 Members

        public int CanRemapLeafFrame()
        {
            throw new NotImplementedException();
        }

        public int IsCurrentException()
        {
            throw new NotImplementedException();
        }

        public int RemapLeafFrame()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugQueryEngine2 Members

        public int GetEngineInterface(out object ppUnk)
        {
            return _program.GetEngineInterface(out ppUnk);
        }

        #endregion
    }
}
