namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using Path = System.IO.Path;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Tvl.VisualStudio.Language.Java.Debugger.Extensions;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Shell;
    using System.ServiceModel.Channels;
    using System.ServiceModel;
    using EventResetMode = System.Threading.EventResetMode;
    using EventWaitHandle = System.Threading.EventWaitHandle;
    using Tvl.VisualStudio.Language.Java.Debugger.Events;
    using Tvl.VisualStudio.Language.Java.Debugger.Collections;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public partial class JavaDebugProgram
        : IDebugProgram3
        , IDebugProgram2
        , IDebugProgramEx2
        , IDebugProgramHost2
        , IDebugEngineProgram2
        , IDebugProgramNode2
        , IDebugProviderProgramNode2
        , IDebugProgramNodeAttach2
        , IDebugProgramEngines2
        , IDebugQueryEngine2
    {
        private readonly IDebugProcess2 _process;
        private Guid? _programId;
        private JavaDebugEngine _debugEngine;
        private IDebugEventCallback2 _callback;

        private System.Threading.EventWaitHandle _ipcHandle;
        private JvmDebugSessionService.IJvmDebugSessionService _sessionService;
        private JvmEventsService.IJvmEventsService _eventsService;
        private JvmToolsService.IJvmToolsInterfaceService _toolsService;

        private readonly Dictionary<int, JavaDebugThread> _threads = new Dictionary<int, JavaDebugThread>();

        public JavaDebugProgram(IDebugProcess2 process)
        {
            Contract.Requires<ArgumentNullException>(process != null, "process");

            _process = process;
        }

        public JavaDebugEngine DebugEngine
        {
            get
            {
                return _debugEngine;
            }
        }

        public IDebugProcess2 Process
        {
            get
            {
                return _process;
            }
        }

        public IDebugEventCallback2 Callback
        {
            get
            {
                return _callback;
            }
        }

        internal JvmDebugSessionService.IJvmDebugSessionService SessionService
        {
            get
            {
                return _sessionService;
            }
        }

        internal JvmEventsService.IJvmEventsService EventsService
        {
            get
            {
                return _eventsService;
            }
        }

        internal JvmToolsService.IJvmToolsInterfaceService ToolsService
        {
            get
            {
                return _toolsService;
            }
        }

        internal void InitializeDebuggerChannel(JavaDebugEngine debugEngine, IDebugEventCallback2 callback)
        {
            Contract.Requires<ArgumentNullException>(debugEngine != null, "debugEngine");
            Contract.Requires<ArgumentNullException>(callback != null, "callback");

            _debugEngine = debugEngine;
            _callback = callback;

            _ipcHandle = new EventWaitHandle(false, EventResetMode.ManualReset, string.Format("JavaDebuggerInitHandle{0}", _process.GetPhysicalProcessId().dwProcessId));
            Action waitToInitializeServices = InitializeServicesAfterProcessStarts;
            waitToInitializeServices.BeginInvoke(null, null);

            IDebugEvent2 @event = new DebugProgramCreateEvent(enum_EVENTATTRIBUTES.EVENT_SYNCHRONOUS);
            Guid guid = typeof(IDebugProgramCreateEvent2).GUID;
            callback.Event(DebugEngine, Process, this, null, @event, ref guid, (uint)@event.GetAttributes());
        }

        private void InitializeServicesAfterProcessStarts()
        {
            try
            {
                _ipcHandle.WaitOne();
                _ipcHandle.Dispose();
                _ipcHandle = null;

                CreateSessionServiceClient();
                CreateEventsServiceClient();
                CreateToolsServiceClient();

                _sessionService.Attach();
            }
            catch (Exception e)
            {
                if (ErrorHandler.IsCriticalException(e))
                    throw;
            }
        }

        private void CreateSessionServiceClient()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            var remoteAddress = new EndpointAddress("net.pipe://localhost/Tvl.Java.DebugHost/JvmDebugSessionService/");
            _sessionService = new JvmDebugSessionService.JvmDebugSessionServiceClient(binding, remoteAddress);
        }

        private void CreateEventsServiceClient()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            JvmEventsCallback callback = new JvmEventsCallback(this);
            var callbackInstance = new InstanceContext(callback);
            var remoteAddress = new EndpointAddress("net.pipe://localhost/Tvl.Java.DebugHost/JvmEventsService/");
            _eventsService = new JvmEventsService.JvmEventsServiceClient(callbackInstance, binding, remoteAddress);
            callback.Subscribe();
        }

        private void CreateToolsServiceClient()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.MaxValue,
                SendTimeout = TimeSpan.MaxValue
            };

            var remoteAddress = new EndpointAddress("net.pipe://localhost/Tvl.Java.DebugHost/JvmToolsInterfaceService/");
            _toolsService = new JvmToolsService.JvmToolsInterfaceServiceClient(binding, remoteAddress);
        }

        #region IDebugProgram2 Members

        int IDebugProgram2.Attach(IDebugEventCallback2 pCallback)
        {
            throw new NotSupportedException("Per MSDN, a debug engine never calls this method to attach to a program.");
        }

        public int CanDetach()
        {
            return VSConstants.S_OK;
        }

        public int CauseBreak()
        {
            throw new NotImplementedException();
        }

        public int Continue(IDebugThread2 pThread)
        {
            throw new NotImplementedException();
        }

        public int Detach()
        {
            throw new NotImplementedException();
        }

        public int EnumCodeContexts(IDebugDocumentPosition2 pDocPos, out IEnumDebugCodeContexts2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int EnumCodePaths(string pszHint, IDebugCodeContext2 pStart, IDebugStackFrame2 pFrame, int fSource, out IEnumCodePaths2 ppEnum, out IDebugCodeContext2 ppSafety)
        {
            throw new NotImplementedException();
        }

        public int EnumModules(out IEnumDebugModules2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int EnumThreads(out IEnumDebugThreads2 ppEnum)
        {
            ppEnum = new EnumDebugThreads(_threads.Values);
            throw new NotImplementedException();
        }

        public int Execute()
        {
            throw new NotImplementedException();
        }

        public int GetDebugProperty(out IDebugProperty2 ppProperty)
        {
            throw new NotImplementedException();
        }

        public int GetDisassemblyStream(enum_DISASSEMBLY_STREAM_SCOPE dwScope, IDebugCodeContext2 pCodeContext, out IDebugDisassemblyStream2 ppDisassemblyStream)
        {
            throw new NotImplementedException();
        }

        public int GetENCUpdate(out object ppUpdate)
        {
            throw new NotImplementedException();
        }

        public int GetEngineInfo(out string pbstrEngine, out Guid pguidEngine)
        {
            pguidEngine = JavaDebuggerConstants.JavaDebugEngineGuid;
            IVsDebugger2 shellDebugger = (IVsDebugger2)Package.GetGlobalService(typeof(SVsShellDebugger));
            return shellDebugger.GetEngineName(ref pguidEngine, out pbstrEngine);
        }

        public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
        {
            throw new NotImplementedException();
        }

        public int GetName(out string pbstrName)
        {
            return GetProgramName(out pbstrName);
        }

        public int GetProcess(out IDebugProcess2 ppProcess)
        {
            ppProcess = _process;
            return VSConstants.S_OK;
        }

        public int GetProgramId(out Guid pguidProgramId)
        {
            pguidProgramId = _programId.Value;
            return VSConstants.S_OK;
        }

        public int Step(IDebugThread2 pThread, enum_STEPKIND sk, enum_STEPUNIT Step)
        {
            throw new NotImplementedException();
        }

        public int Terminate()
        {
            throw new NotImplementedException();
        }

        public int WriteDump(enum_DUMPTYPE DUMPTYPE, string pszDumpUrl)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProgram3 Members

        public int ExecuteOnThread(IDebugThread2 pThread)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProgramEx2 Members

        public int Attach(IDebugEventCallback2 pCallback, uint dwReason, IDebugSession2 pSession)
        {
            throw new NotImplementedException();
        }

        public int GetProgramNode(out IDebugProgramNode2 ppProgramNode)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProgramHost2 Members

        public int GetHostId(AD_PROCESS_ID[] pProcessId)
        {
            throw new NotImplementedException();
        }

        public int GetHostMachineName(out string pbstrHostMachineName)
        {
            throw new NotImplementedException();
        }

        int IDebugProgramHost2.GetHostName(uint dwType, out string pbstrHostName)
        {
            return GetHostName((enum_GETHOSTNAME_TYPE)dwType, out pbstrHostName);
        }

        #endregion

        #region IDebugEngineProgram2 Members

        public int Stop()
        {
            throw new NotImplementedException();
        }

        public int WatchForExpressionEvaluationOnThread(IDebugProgram2 pOriginatingProgram, uint dwTid, uint dwEvalFlags, IDebugEventCallback2 pExprCallback, int fWatch)
        {
            throw new NotImplementedException();
        }

        public int WatchForThreadStep(IDebugProgram2 pOriginatingProgram, uint dwTid, int fWatch, uint dwFrame)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProgramNode2 Members

        [Obsolete]
        int IDebugProgramNode2.Attach_V7(IDebugProgram2 pMDMProgram, IDebugEventCallback2 pCallback, uint dwReason)
        {
            throw new NotSupportedException();
        }

        [Obsolete]
        int IDebugProgramNode2.DetachDebugger_V7()
        {
            throw new NotSupportedException();
        }

        [Obsolete]
        int IDebugProgramNode2.GetHostMachineName_V7(out string pbstrHostMachineName)
        {
            throw new NotSupportedException();
        }

        public int GetHostName(enum_GETHOSTNAME_TYPE dwHostNameType, out string pbstrHostName)
        {
            return _process.GetName((enum_GETNAME_TYPE)dwHostNameType, out pbstrHostName);
        }

        public int GetHostPid(AD_PROCESS_ID[] pHostProcessId)
        {
            return _process.GetPhysicalProcessId(pHostProcessId);
        }

        public int GetProgramName(out string pbstrProgramName)
        {
            // in the future, this should return the name of the jar or startup class
            int result = GetHostName(enum_GETHOSTNAME_TYPE.GHN_FILE_NAME, out pbstrProgramName);
            if (result == VSConstants.S_OK)
                pbstrProgramName = Path.GetFileName(pbstrProgramName);

            return result;
        }

        #endregion

        #region IDebugProviderProgramNode2 Members

        public int UnmarshalDebuggeeInterface(ref Guid riid, out IntPtr ppvObject)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugProgramNodeAttach2 Members

        /// <summary>
        /// Attaches to the associated program or defers the attach process to the <see cref="IDebugEngine2.Attach"/> method.
        /// </summary>
        /// <param name="guidProgramId">GUID to assign to the associated program.</param>
        /// <returns>
        /// If successful, returns S_OK. Returns S_FALSE if the <see cref="IDebugEngine2.Attach"/> method should
        /// not be called. Otherwise, returns an error code.
        /// </returns>
        /// <remarks>
        /// This method is called during the attach process, before the IDebugEngine2::Attach method is called.
        /// The OnAttach method can perform the attach process itself (in which case, this method returns S_FALSE)
        /// or defer the attach process to the IDebugEngine2::Attach method (the OnAttach method returns S_OK).
        /// In either event, the OnAttach method can set the GUID of the program being debugged to the given GUID.
        /// </remarks>
        public int OnAttach(ref Guid guidProgramId)
        {
            _programId = guidProgramId;
            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugProgramEngines2 Members

        public int EnumPossibleEngines(uint celtBuffer, Guid[] rgguidEngines, ref uint pceltEngines)
        {
            pceltEngines = 1;
            if (celtBuffer < pceltEngines)
                return JavaDebuggerConstants.E_INSUFFICIENT_BUFFER;

            if (rgguidEngines == null || rgguidEngines.Length < 1)
                throw new ArgumentException("rgguidEngines");

            rgguidEngines[0] = JavaDebuggerConstants.JavaDebugEngineGuid;
            return VSConstants.S_OK;
        }

        public int SetEngine(ref Guid guidEngine)
        {
            Contract.Assert(guidEngine == JavaDebuggerConstants.JavaDebugEngineGuid);
            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugQueryEngine2 Members

        public int GetEngineInterface(out object ppUnk)
        {
            ppUnk = _debugEngine;
            return ppUnk != null ? VSConstants.S_OK : VSConstants.E_FAIL;
        }

        #endregion
    }
}
