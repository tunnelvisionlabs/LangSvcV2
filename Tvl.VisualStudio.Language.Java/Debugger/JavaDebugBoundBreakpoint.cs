namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;

    [ComVisible(true)]
    public class JavaDebugBoundBreakpoint : IDebugBoundBreakpoint3, IDebugBoundBreakpoint2
    {
        private readonly JavaDebugPendingBreakpoint _pendingBreakpoint;
        private readonly DebugBreakpointResolution _resolution;

        public JavaDebugBoundBreakpoint(JavaDebugPendingBreakpoint pendingBreakpoint, DebugBreakpointResolution resolution)
        {
            Contract.Requires<ArgumentNullException>(pendingBreakpoint != null, "pendingBreakpoint");
            Contract.Requires<ArgumentNullException>(resolution != null, "resolution");

            _pendingBreakpoint = pendingBreakpoint;
            _resolution = resolution;
        }

        #region IDebugBoundBreakpoint2 Members

        public int Delete()
        {
            throw new NotImplementedException();
        }

        public int Enable(int fEnable)
        {
            throw new NotImplementedException();
        }

        public int GetBreakpointResolution(out IDebugBreakpointResolution2 resolution)
        {
            resolution = _resolution;
            return VSConstants.S_OK;
        }

        public int GetHitCount(out uint pdwHitCount)
        {
            throw new NotImplementedException();
        }

        public int GetPendingBreakpoint(out IDebugPendingBreakpoint2 pendingBreakpoint)
        {
            pendingBreakpoint = _pendingBreakpoint;
            return VSConstants.S_OK;
        }

        public int GetState(enum_BP_STATE[] pState)
        {
            if (pState == null)
                throw new ArgumentNullException("pState");
            if (pState.Length == 0)
                throw new ArgumentException();


            //pState[0] = enum_BP_STATE.
            throw new NotImplementedException();
        }

        public int SetCondition(BP_CONDITION bpCondition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the hit count for the bound breakpoint.
        /// </summary>
        /// <param name="dwHitCount">The hit count to set.</param>
        /// <returns>
        /// If successful, returns S_OK; otherwise, returns an error code. Returns E_BP_DELETED
        /// if the state of the bound breakpoint object is set to BPS_DELETED (part of the BP_STATE
        /// enumeration).
        /// </returns>
        /// <remarks>
        /// The hit count is the number of times this breakpoint has fired during the current run of the session.
        /// This method is typically called by the debug engine to update the current hit count on this breakpoint.
        /// </remarks>
        public int SetHitCount(uint dwHitCount)
        {
            throw new NotImplementedException();
        }

        public int SetPassCount(BP_PASSCOUNT bpPassCount)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugBoundBreakpoint3 Members

        public int SetTracepoint(string bpBstrTracepoint, enum_BP_FLAGS bpFlags)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
