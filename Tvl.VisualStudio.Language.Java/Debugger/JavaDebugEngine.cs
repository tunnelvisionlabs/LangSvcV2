namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using EnumDebugPrograms = Tvl.VisualStudio.Language.Java.Debugger.Collections.EnumDebugPrograms;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using System.Globalization;
    using Tvl.VisualStudio.Language.Java.Debugger.Events;
    using Tvl.VisualStudio.Language.Java.Debugger.Extensions;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.Java.DebugInterface.Request;
    using Tvl.Java.DebugInterface;
    using Tvl.Extensions;
    using System.Threading.Tasks;

    [ComVisible(true)]
    [Guid(JavaDebuggerConstants.JavaDebugEngineGuidString)]
    public class JavaDebugEngine
        : IDebugEngine3
        , IDebugEngine2
        //, IDebugEngineLaunch2
    {
        private CultureInfo _culture;
        private string _registryRoot;
        private string[] _symbolSearchPath;
        private string[] _symbolCachePath;

        private readonly List<IDebugProgram2> _programs = new List<IDebugProgram2>();

        public JavaDebugEngine()
        {
        }

        #region IDebugEngine2 Members

        public int Attach(IDebugProgram2[] rgpPrograms, IDebugProgramNode2[] rgpProgramNodes, uint celtPrograms, IDebugEventCallback2 pCallback, enum_ATTACH_REASON dwReason)
        {
            if (celtPrograms == 0)
                return VSConstants.S_OK;

            if (pCallback == null)
                throw new ArgumentNullException("pCallback");
            if (rgpPrograms == null || rgpPrograms.Length < celtPrograms)
                throw new ArgumentException();
            if (rgpProgramNodes == null || rgpProgramNodes.Length < celtPrograms)
                throw new ArgumentException();

            if (celtPrograms > 1)
                throw new NotImplementedException();

            if (dwReason != enum_ATTACH_REASON.ATTACH_REASON_LAUNCH)
                throw new NotImplementedException();

            JavaDebugProgram program = rgpProgramNodes[0] as JavaDebugProgram;
            if (program == null)
                throw new NotSupportedException();

            lock (_programs)
            {
                _programs.Add(program);
            }

            DebugEvent @event = new DebugEngineCreateEvent(enum_EVENTATTRIBUTES.EVENT_ASYNCHRONOUS, this);
            pCallback.Event(this, program.GetProcess(), program, null, @event);

            program.InitializeDebuggerChannel(this, pCallback);
            return VSConstants.S_OK;
        }

        public int CauseBreak()
        {
            throw new NotImplementedException();
        }

        public int ContinueFromSynchronousEvent(IDebugEvent2 pEvent)
        {
            if (pEvent == null)
                throw new ArgumentNullException("pEvent");
            if (!(pEvent is DebugEvent))
                return VSConstants.E_INVALIDARG;

            if (pEvent is IDebugEngineCreateEvent2)
                return VSConstants.S_OK;

            IPropertyOwner propertyOwner = pEvent as IPropertyOwner;
            if (propertyOwner != null)
            {
                SuspendPolicy suspendPolicy;
                if (propertyOwner.Properties.TryGetProperty(typeof(SuspendPolicy), out suspendPolicy))
                {
                    switch (suspendPolicy)
                    {
                    case SuspendPolicy.All:
                        IVirtualMachine virtualMachine = propertyOwner.Properties.GetProperty<IVirtualMachine>(typeof(IVirtualMachine));
                        Task.Factory.StartNew(virtualMachine.Resume).HandleNonCriticalExceptions();
                        break;

                    case SuspendPolicy.EventThread:
                        IThreadReference thread = propertyOwner.Properties.GetProperty<IThreadReference>(typeof(IThreadReference));
                        Task.Factory.StartNew(thread.Resume).HandleNonCriticalExceptions();
                        break;

                    case SuspendPolicy.None:
                        break;
                    }
                }
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Creates a pending breakpoint in the debug engine (DE).
        /// </summary>
        /// <param name="breakpointRequest">An IDebugBreakpointRequest2 object that describes the pending breakpoint to create.</param>
        /// <param name="pendingBreakpoint">Returns an IDebugPendingBreakpoint2 object that represents the pending breakpoint.</param>
        /// <returns>
        /// If successful, returns S_OK; otherwise, returns an error code. Typically returns E_FAIL if the pBPRequest parameter
        /// does not match any language supported by the DE of if the pBPRequest parameter is invalid or incomplete.
        /// </returns>
        /// <remarks>
        /// A pending breakpoint is essentially a collection of all the information needed to bind a breakpoint to code. The
        /// pending breakpoint returned from this method is not bound to code until the IDebugPendingBreakpoint2.Bind method
        /// is called.
        /// 
        /// For each pending breakpoint the user sets, the session debug manager (SDM) calls this method in each attached DE.
        /// It is up to the DE to verify that the breakpoint is valid for programs running in that DE.
        /// 
        /// When the user sets a breakpoint on a line of code, the DE is free to bind the breakpoint to the closest line in
        /// the document that corresponds to this code. This makes it possible for the user to set a breakpoint on the first
        /// line of a multi-line statement, but bind it on the last line (where all the code is attributed in the debug
        /// information).
        /// </remarks>
        public int CreatePendingBreakpoint(IDebugBreakpointRequest2 breakpointRequest, out IDebugPendingBreakpoint2 pendingBreakpoint)
        {
            pendingBreakpoint = null;

            BreakpointRequestInfo requestInfo = new BreakpointRequestInfo(breakpointRequest);
            if (requestInfo.LanguageGuid != Constants.JavaLanguageGuid)
                return VSConstants.E_FAIL;

            throw new NotImplementedException();
        }

        public int DestroyProgram(IDebugProgram2 program)
        {
            JavaDebugProgram javaProgram = program as JavaDebugProgram;
            if (javaProgram == null)
                return VSConstants.E_INVALIDARG;

            lock (_programs)
            {
                _programs.Remove(program);
            }

            return VSConstants.S_OK;
        }

        public int EnumPrograms(out IEnumDebugPrograms2 programs)
        {
            lock (_programs)
            {
                programs = new EnumDebugPrograms(_programs.ToArray());
            }

            return VSConstants.S_OK;
        }

        public int GetEngineId(out Guid pguidEngine)
        {
            pguidEngine = JavaDebuggerConstants.JavaDebugEngineGuid;
            return VSConstants.S_OK;
        }

        public int RemoveAllSetExceptions(ref Guid guidType)
        {
            throw new NotImplementedException();
        }

        public int RemoveSetException(EXCEPTION_INFO[] pException)
        {
            throw new NotImplementedException();
        }

        public int SetException(EXCEPTION_INFO[] pException)
        {
            throw new NotImplementedException();
        }

        public int SetLocale(ushort wLangID)
        {
            _culture = CultureInfo.GetCultureInfo(wLangID);
            return VSConstants.S_OK;
        }

        public int SetMetric(string pszMetric, object varValue)
        {
            throw new NotImplementedException();
        }

        public int SetRegistryRoot(string pszRegistryRoot)
        {
            _registryRoot = pszRegistryRoot;
            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugEngine3 Members

        public int LoadSymbols()
        {
            return VSConstants.S_OK;
        }

        public int SetAllExceptions(enum_EXCEPTION_STATE dwState)
        {
            throw new NotImplementedException();
        }

        public int SetEngineGuid(ref Guid guidEngine)
        {
            Contract.Assert(guidEngine == JavaDebuggerConstants.JavaDebugEngineGuid);
            return VSConstants.S_OK;
        }

        public int SetJustMyCodeState(int fUpdate, uint dwModules, JMC_CODE_SPEC[] rgJMCSpec)
        {
            throw new NotImplementedException();
        }

        public int SetSymbolPath(string szSymbolSearchPath, string szSymbolCachePath, uint flags)
        {
            if (szSymbolSearchPath == null)
                throw new ArgumentNullException("szSymbolSearchPath");
            if (szSymbolCachePath == null)
                throw new ArgumentNullException("szSymbolCachePath");

            _symbolSearchPath = szSymbolSearchPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            _symbolCachePath = szSymbolCachePath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //_symbolPathFlags = flags; // flags is always 0
            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugEngineLaunch2 Members

        public int CanTerminateProcess(IDebugProcess2 pProcess)
        {
            throw new NotImplementedException();
        }

        public int LaunchSuspended(string pszServer, IDebugPort2 pPort, string pszExe, string pszArgs, string pszDir, string bstrEnv, string pszOptions, enum_LAUNCH_FLAGS dwLaunchFlags, uint hStdInput, uint hStdOutput, uint hStdError, IDebugEventCallback2 pCallback, out IDebugProcess2 ppProcess)
        {
            throw new NotImplementedException();
        }

        public int ResumeProcess(IDebugProcess2 pProcess)
        {
            throw new NotImplementedException();
        }

        public int TerminateProcess(IDebugProcess2 pProcess)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
