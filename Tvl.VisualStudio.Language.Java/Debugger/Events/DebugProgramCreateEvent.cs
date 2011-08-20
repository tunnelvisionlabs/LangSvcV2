namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugProgramCreateEvent : DebugEvent, IDebugProgramCreateEvent2
    {
        public DebugProgramCreateEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
