namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using Tvl.Java.DebugInterface;
    using Tvl.VisualStudio.Language.Java.Debugger.Extensions;

    [ComVisible(true)]
    public class JavaDebugExceptionEvent : DebugEvent, IDebugExceptionEvent2
    {
        private readonly JavaDebugProgram _program;
        private readonly IObjectReference _exceptionObject;

        public JavaDebugExceptionEvent(enum_EVENTATTRIBUTES attributes, JavaDebugProgram program, IObjectReference exceptionObject)
            : base(attributes)
        {
            Contract.Requires<ArgumentNullException>(program != null, "program");
            Contract.Requires<ArgumentNullException>(exceptionObject != null, "exceptionObject");

            _program = program;
            _exceptionObject = exceptionObject;
        }

        public override Guid EventGuid
        {
            get
            {
                return typeof(IDebugExceptionEvent2).GUID;
            }
        }

        /// <summary>
        /// Determines whether or not the debug engine (DE) supports the option of passing this exception to the program being debugged when execution resumes.
        /// </summary>
        /// <returns>Returns either S_OK (the exception can be passed to the program) or S_FALSE (the exception cannot be passed on).</returns>
        /// <remarks>
        /// The DE must have a default action for passing to the debuggee. The IDE may receive the IDebugExceptionEvent2 event
        /// and call the IDebugProcess3.Continue method without calling the CanPassToDebuggee method. Therefore, the DE should
        /// have a default case for passing the exception on or not.
        /// </remarks>
        public int CanPassToDebuggee()
        {
            return VSConstants.S_OK;
        }

        public int GetException(EXCEPTION_INFO[] pExceptionInfo)
        {
            if (pExceptionInfo == null)
                throw new ArgumentNullException("pExceptionInfo");
            if (pExceptionInfo.Length == 0)
                throw new ArgumentException();

            pExceptionInfo[0].bstrExceptionName = _exceptionObject.GetReferenceType().GetName();
            pExceptionInfo[0].bstrProgramName = _program.GetName();
            pExceptionInfo[0].dwCode = 0;
            pExceptionInfo[0].dwState = enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE;
            pExceptionInfo[0].guidType = Guid.Empty;
            pExceptionInfo[0].pProgram = _program;
            return VSConstants.S_OK;
        }

        public int GetExceptionDescription(out string pbstrDescription)
        {
            IClassType exceptionType = (IClassType)_exceptionObject.GetReferenceType();

#if false
            IMethod tostring = exceptionType.GetConcreteMethod("ToString", "()Ljava/lang/String;");
            IValue result = _exceptionObject.InvokeMethod(null, tostring, InvokeOptions.None);
            IStringReference stringReference = result as IStringReference;
            if (stringReference != null)
            {
                pbstrDescription = stringReference.GetValue();
                stringReference.EnableCollection();
            }
#else
            pbstrDescription = exceptionType.GetName();
#endif

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Specifies whether the exception should be passed on to the program being debugged when execution resumes, or if the exception should be discarded.
        /// </summary>
        /// <param name="fPass">[in] Nonzero (TRUE) if the exception should be passed on to the program being debugged when execution resumes, or zero (FALSE) if the exception should be discarded.</param>
        /// <returns>If successful, returns S_OK; otherwise, returns an error code.</returns>
        /// <remarks>
        /// Calling this method does not actually cause any code to be executed in the program being debugged.
        /// The call is merely to set the state for the next code execution. For example, calls to the
        /// IDebugExceptionEvent2.CanPassToDebuggee method may return S_OK with the EXCEPTION_INFO.dwState field
        /// set to EXCEPTION_STOP_SECOND_CHANCE.
        /// 
        /// The IDE may receive the IDebugExceptionEvent2 event and call the IDebugProgram2.Continue method.
        /// The debug engine (DE) should have a default behavior to handle the case if the PassToDebuggee
        /// method is not called.
        /// </remarks>
        public int PassToDebuggee(int fPass)
        {
            if (fPass == 0)
                throw new NotImplementedException();

            return VSConstants.S_OK;
        }
    }
}
