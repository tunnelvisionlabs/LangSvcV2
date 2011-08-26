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
    public class EnumDebugErrorBreakpoints : DebugEnumerator<IEnumDebugErrorBreakpoints2, IDebugErrorBreakpoint2>, IEnumDebugErrorBreakpoints2
    {
        public EnumDebugErrorBreakpoints(IEnumerable<IDebugErrorBreakpoint2> breakpoints)
            : base(breakpoints)
        {
            Contract.Requires(breakpoints != null);
        }

        protected EnumDebugErrorBreakpoints(IDebugErrorBreakpoint2[] elements, int currentIndex)
            : base(elements, currentIndex)
        {
        }

        protected override IEnumDebugErrorBreakpoints2 CreateClone(IDebugErrorBreakpoint2[] elements, int currentIndex)
        {
            return new EnumDebugErrorBreakpoints(elements, currentIndex);
        }
    }
}
