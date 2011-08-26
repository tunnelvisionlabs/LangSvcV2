namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Debugger.Interop;

    [ComVisible(true)]
    public class DebugExceptionEvent : DebugEvent, IDebugExceptionEvent2
    {
        public DebugExceptionEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
            throw new NotImplementedException();
        }

        public override Guid EventGuid
        {
            get
            {
                return typeof(IDebugExceptionEvent2).GUID;
            }
        }

        public int CanPassToDebuggee()
        {
            throw new NotImplementedException();
        }

        public int GetException(EXCEPTION_INFO[] pExceptionInfo)
        {
            throw new NotImplementedException();
        }

        public int GetExceptionDescription(out string pbstrDescription)
        {
            throw new NotImplementedException();
        }

        public int PassToDebuggee(int fPass)
        {
            throw new NotImplementedException();
        }
    }
}
