namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using Tvl.VisualStudio.Language.Java.JvmEventsService;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class JavaDebugProcess
        : IDebugProcess3
        , IDebugProcess2
        , IDebugProcessEx2
        , IDebugProcessQueryProperties
        , IDebugProcessSecurity2
        , IDebugQueryEngine2
    {
        #region IDebugProcess2 Members

        public int Attach(IDebugEventCallback2 pCallback, Guid[] rgguidSpecificEngines, uint celtSpecificEngines, int[] rghrEngineAttach)
        {
            throw new NotImplementedException();
        }

        public int CanDetach()
        {
            throw new NotImplementedException();
        }

        public int CauseBreak()
        {
            throw new NotImplementedException();
        }

        public int Detach()
        {
            throw new NotImplementedException();
        }

        public int EnumPrograms(out IEnumDebugPrograms2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int EnumThreads(out IEnumDebugThreads2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int GetAttachedSessionName(out string pbstrSessionName)
        {
            throw new NotImplementedException();
        }

        public int GetInfo(enum_PROCESS_INFO_FIELDS Fields, PROCESS_INFO[] pProcessInfo)
        {
            throw new NotImplementedException();
        }

        public int GetName(enum_GETNAME_TYPE gnType, out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int GetPhysicalProcessId(AD_PROCESS_ID[] pProcessId)
        {
            throw new NotImplementedException();
        }

        public int GetPort(out IDebugPort2 ppPort)
        {
            throw new NotImplementedException();
        }

        public int GetProcessId(out Guid pguidProcessId)
        {
            throw new NotImplementedException();
        }

        public int GetServer(out IDebugCoreServer2 ppServer)
        {
            throw new NotImplementedException();
        }

        public int Terminate()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProcess3 Members


        public int Continue(IDebugThread2 pThread)
        {
            throw new NotImplementedException();
        }

        public int DisableENC(EncUnavailableReason reason)
        {
            throw new NotImplementedException();
        }

        public int Execute(IDebugThread2 pThread)
        {
            throw new NotImplementedException();
        }

        public int GetDebugReason(enum_DEBUG_REASON[] pReason)
        {
            throw new NotImplementedException();
        }

        public int GetENCAvailableState(EncUnavailableReason[] pReason)
        {
            throw new NotImplementedException();
        }

        public int GetEngineFilter(GUID_ARRAY[] pEngineArray)
        {
            throw new NotImplementedException();
        }

        public int GetHostingProcessLanguage(out Guid pguidLang)
        {
            throw new NotImplementedException();
        }

        public int SetHostingProcessLanguage(ref Guid guidLang)
        {
            throw new NotImplementedException();
        }

        public int Step(IDebugThread2 pThread, enum_STEPKIND sk, enum_STEPUNIT Step)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProcessEx2 Members

        public int AddImplicitProgramNodes(ref Guid guidLaunchingEngine, Guid[] rgguidSpecificEngines, uint celtSpecificEngines)
        {
            throw new NotImplementedException();
        }

        public int Attach(IDebugSession2 pSession)
        {
            throw new NotImplementedException();
        }

        public int Detach(IDebugSession2 pSession)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProcessQueryProperties Members

        public int QueryProperties(uint celt, uint[] rgdwPropTypes, object[] rgtPropValues)
        {
            throw new NotImplementedException();
        }

        public int QueryProperty(uint dwPropType, out object pvarPropValue)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProcessSecurity2 Members

        public int GetUserName(out string pbstrUserName)
        {
            throw new NotImplementedException();
        }

        public int QueryCanSafelyAttach()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugQueryEngine2 Members

        public int GetEngineInterface(out object ppUnk)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
