namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugInterceptExceptionCompleteEvent : DebugEvent, IDebugInterceptExceptionCompleteEvent2
    {
        private readonly ulong _cookie;

        public DebugInterceptExceptionCompleteEvent(enum_EVENTATTRIBUTES attributes, ulong cookie)
            : base(attributes)
        {
            _cookie = cookie;
        }

        public int GetInterceptCookie(out ulong pqwCookie)
        {
            pqwCookie = _cookie;
            return VSConstants.S_OK;
        }
    }
}
