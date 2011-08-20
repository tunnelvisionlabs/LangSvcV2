namespace Tvl.VisualStudio.Language.Java.Debugger.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class DebugBreakpointBoundEvent : DebugEvent, IDebugBreakpointBoundEvent2
    {
        private readonly IDebugPendingBreakpoint2 _pendingBreakpoint;
        private readonly IEnumDebugBoundBreakpoints2 _boundBreakpoints;

        public DebugBreakpointBoundEvent(enum_EVENTATTRIBUTES attributes, IDebugPendingBreakpoint2 pendingBreakpoint, IEnumDebugBoundBreakpoints2 boundBreakpoints)
            : base(attributes)
        {
            Contract.Requires<ArgumentNullException>(pendingBreakpoint != null, "pendingBreakpoint");
            Contract.Requires<ArgumentNullException>(boundBreakpoints != null, "boundBreakpoints");

            _pendingBreakpoint = pendingBreakpoint;
            _boundBreakpoints = boundBreakpoints;
        }

        public int EnumBoundBreakpoints(out IEnumDebugBoundBreakpoints2 ppEnum)
        {
            ppEnum = _boundBreakpoints;
            return VSConstants.S_OK;
        }

        public int GetPendingBreakpoint(out IDebugPendingBreakpoint2 ppPendingBP)
        {
            ppPendingBP = _pendingBreakpoint;
            return VSConstants.S_OK;
        }
    }
}
