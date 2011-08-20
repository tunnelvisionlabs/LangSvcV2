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
    public class EnumDebugProcesses : DebugEnumerator<IEnumDebugProcesses2, IDebugProcess2>, IEnumDebugProcesses2
    {
        public EnumDebugProcesses(IEnumerable<IDebugProcess2> processes)
            : base(processes)
        {
            Contract.Requires(processes != null);
        }
    }
}
