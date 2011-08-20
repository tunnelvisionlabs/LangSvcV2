namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugProcessCreateEvent : DebugEvent, IDebugProcessCreateEvent2
    {
        public DebugProcessCreateEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
