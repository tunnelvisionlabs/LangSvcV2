namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugReturnValueEvent : DebugEvent, IDebugReturnValueEvent2
    {
        private readonly IDebugProperty2 _returnValue;

        public DebugReturnValueEvent(enum_EVENTATTRIBUTES attributes, IDebugProperty2 returnValue)
            : base(attributes)
        {
            Contract.Requires<ArgumentNullException>(returnValue != null, "returnValue");

            _returnValue = returnValue;
        }

        public int GetReturnValue(out IDebugProperty2 ppReturnValue)
        {
            ppReturnValue = _returnValue;
            return VSConstants.S_OK;
        }
    }
}
