namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugPropertyCreateEvent : DebugEvent, IDebugPropertyCreateEvent2
    {
        private readonly IDebugProperty2 _property;

        public DebugPropertyCreateEvent(enum_EVENTATTRIBUTES attributes, IDebugProperty2 property)
            : base(attributes)
        {
            Contract.Requires<ArgumentNullException>(property != null, "property");

            _property = property;
        }

        public int GetDebugProperty(out IDebugProperty2 ppProperty)
        {
            ppProperty = _property;
            return VSConstants.S_OK;
        }
    }
}
