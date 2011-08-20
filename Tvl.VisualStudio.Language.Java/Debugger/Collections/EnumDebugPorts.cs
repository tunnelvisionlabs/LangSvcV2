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
    public class EnumDebugPorts : DebugEnumerator<IEnumDebugPorts2, IDebugPort2>, IEnumDebugPorts2
    {
        public EnumDebugPorts(IEnumerable<IDebugPort2> ports)
            : base(ports)
        {
            Contract.Requires(ports != null);
        }
    }
}
