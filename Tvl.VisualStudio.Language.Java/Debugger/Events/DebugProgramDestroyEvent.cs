namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using Microsoft.VisualStudio;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugProgramDestroyEvent : DebugEvent, IDebugProgramDestroyEvent2
    {
        private readonly uint _exitCode;

        public DebugProgramDestroyEvent(enum_EVENTATTRIBUTES attributes, uint exitCode)
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
