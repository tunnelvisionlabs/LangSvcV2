namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugNoSymbolsEvent : DebugEvent, IDebugNoSymbolsEvent2
    {
        public DebugNoSymbolsEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }
    }
}
