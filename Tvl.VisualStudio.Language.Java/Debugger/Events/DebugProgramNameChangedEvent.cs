namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugProgramNameChangedEvent : DebugEvent, IDebugProgramNameChangedEvent2
    {
        public DebugProgramNameChangedEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
