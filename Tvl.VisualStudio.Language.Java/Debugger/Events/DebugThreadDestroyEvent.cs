namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugThreadDestroyEvent : DebugEvent, IDebugThreadDestroyEvent2
    {
        private readonly uint _exitCode;

        public DebugThreadDestroyEvent(enum_EVENTATTRIBUTES attributes, uint exitCode)
            : base(attributes)
        {
            _exitCode = exitCode;
        }

        public int GetExitCode(out uint pdwExit)
        {
            pdwExit = _exitCode;
            return VSConstants.S_OK;
        }
    }
}
