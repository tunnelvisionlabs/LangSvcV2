namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugBreakEvent : DebugEvent, IDebugBreakEvent2
    {
        public DebugBreakEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
