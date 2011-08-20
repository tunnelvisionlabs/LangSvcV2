namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugThreadNameChangedEvent : DebugEvent, IDebugThreadNameChangedEvent2
    {
        public DebugThreadNameChangedEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
