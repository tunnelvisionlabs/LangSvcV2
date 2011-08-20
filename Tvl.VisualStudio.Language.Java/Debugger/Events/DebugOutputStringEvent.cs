namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugOutputStringEvent : DebugEvent, IDebugOutputStringEvent2
    {
        private readonly string _message;

        public DebugOutputStringEvent(enum_EVENTATTRIBUTES attributes, string message)
            : base(attributes)
        {
            Contract.Requires<ArgumentNullException>(message != null, "message");

            _message = message;
        }

        public int GetString(out string pbstrString)
        {
            pbstrString = _message;
            return VSConstants.S_OK;
        }
    }
}
