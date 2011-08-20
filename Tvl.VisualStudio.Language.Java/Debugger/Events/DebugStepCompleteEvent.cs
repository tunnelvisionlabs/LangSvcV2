namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugStepCompleteEvent : DebugEvent, IDebugStepCompleteEvent2
    {
        public DebugStepCompleteEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
