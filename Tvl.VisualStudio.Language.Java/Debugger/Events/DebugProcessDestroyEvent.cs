namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugProcessDestroyEvent : DebugEvent, IDebugProcessDestroyEvent2
    {
        public DebugProcessDestroyEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
