namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugCanStopEvent : DebugEvent, IDebugCanStopEvent2
    {
        public DebugCanStopEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
            throw new NotImplementedException();
        }

        public int CanStop(int fCanStop)
        {
            throw new NotImplementedException();
        }

        public int GetCodeContext(out IDebugCodeContext2 ppCodeContext)
        {
            throw new NotImplementedException();
        }

        public int GetDocumentContext(out IDebugDocumentContext2 ppDocCxt)
        {
            throw new NotImplementedException();
        }

        public int GetReason(enum_CANSTOP_REASON[] pcr)
        {
            throw new NotImplementedException();
        }
    }
}
