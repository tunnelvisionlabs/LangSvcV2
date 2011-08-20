using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Tvl.VisualStudio.Language.Java.Debugger
{
    [ComVisible(true)]
    public class JavaDebugPort : IDebugPort2, IDebugPortEx2, IDebugPortNotify2
    {
        #region IDebugPort2 Members

        public int EnumProcesses(out IEnumDebugProcesses2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int GetPortId(out Guid pguidPort)
        {
            throw new NotImplementedException();
        }

        public int GetPortName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int GetPortRequest(out IDebugPortRequest2 ppRequest)
        {
            throw new NotImplementedException();
        }

        public int GetPortSupplier(out IDebugPortSupplier2 ppSupplier)
        {
            throw new NotImplementedException();
        }

        public int GetProcess(AD_PROCESS_ID ProcessId, out IDebugProcess2 ppProcess)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugPortEx2 Members

        public int CanTerminateProcess(IDebugProcess2 pPortProcess)
        {
            throw new NotImplementedException();
        }

        public int GetPortProcessId(out uint pdwProcessId)
        {
            throw new NotImplementedException();
        }

        public int GetProgram(IDebugProgramNode2 pProgramNode, out IDebugProgram2 ppProgram)
        {
            throw new NotImplementedException();
        }

        public int LaunchSuspended(string pszExe, string pszArgs, string pszDir, string bstrEnv, uint hStdInput, uint hStdOutput, uint hStdError, out IDebugProcess2 ppPortProcess)
        {
            throw new NotImplementedException();
        }

        public int ResumeProcess(IDebugProcess2 pPortProcess)
        {
            throw new NotImplementedException();
        }

        public int TerminateProcess(IDebugProcess2 pPortProcess)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugPortNotify2 Members

        public int AddProgramNode(IDebugProgramNode2 pProgramNode)
        {
            throw new NotImplementedException();
        }

        public int RemoveProgramNode(IDebugProgramNode2 pProgramNode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
