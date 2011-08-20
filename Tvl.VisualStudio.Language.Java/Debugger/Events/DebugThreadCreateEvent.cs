namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugThreadCreateEvent : DebugEvent, IDebugThreadCreateEvent2
    {
        public DebugThreadCreateEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
