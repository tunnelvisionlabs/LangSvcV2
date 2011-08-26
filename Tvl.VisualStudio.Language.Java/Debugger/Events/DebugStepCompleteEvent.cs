namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Debugger.Interop;

    [ComVisible(true)]
    public class DebugStepCompleteEvent : DebugEvent, IDebugStepCompleteEvent2
    {
        public DebugStepCompleteEvent(enum_EVENTATTRIBUTES attributes)
            : base(attributes)
        {
        }

        public override Guid EventGuid
        {
            get
            {
                return typeof(IDebugStepCompleteEvent2).GUID;
            }
        }
    }
}
