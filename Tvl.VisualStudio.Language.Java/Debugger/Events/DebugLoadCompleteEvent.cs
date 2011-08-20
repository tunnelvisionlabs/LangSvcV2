namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This event is sent by the debug engine (DE) to the session debug manager (SDM) when a program
    /// is loaded, but before any code is executed.
    /// </summary>
    [ComVisible(true)]
    public class DebugLoadCompleteEvent : DebugEvent, IDebugLoadCompleteEvent2
    {
        public DebugLoadCompleteEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
            Contract.Requires<ArgumentException>((attributes & enum_EVENTATTRIBUTES.EVENT_STOPPING) != 0);
        }
    }
}
