namespace Tvl.VisualStudio.Language.Java.Debugger.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class EnumDebugBoundBreakpoints : DebugEnumerator<IEnumDebugBoundBreakpoints2, IDebugBoundBreakpoint2>, IEnumDebugBoundBreakpoints2
    {
        public EnumDebugBoundBreakpoints(IEnumerable<IDebugBoundBreakpoint2> breakpoints)
            : base(breakpoints)
        {
            Contract.Requires(breakpoints != null);
        }

        protected EnumDebugBoundBreakpoints(IDebugBoundBreakpoint2[] elements, int currentIndex)
            : base(elements, currentIndex)
        {
        }

        protected override IEnumDebugBoundBreakpoints2 CreateClone(IDebugBoundBreakpoint2[] elements, int currentIndex)
        {
            return new EnumDebugBoundBreakpoints(elements, currentIndex);
        }
    }
}
