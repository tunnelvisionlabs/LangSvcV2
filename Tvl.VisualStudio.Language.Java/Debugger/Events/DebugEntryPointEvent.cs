namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugEntryPointEvent : DebugEvent, IDebugEntryPointEvent2
    {
        public DebugEntryPointEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
