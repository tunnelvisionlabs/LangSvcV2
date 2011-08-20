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
    public class EnumDebugPropertyInfo : DebugEnumerator<IEnumDebugPropertyInfo2, DEBUG_PROPERTY_INFO>, IEnumDebugPropertyInfo2
    {
        public EnumDebugPropertyInfo(IEnumerable<DEBUG_PROPERTY_INFO> propertyInfo)
            : base(propertyInfo)
        {
            Contract.Requires(propertyInfo != null);
        }

        int IEnumDebugPropertyInfo2.Next(uint celt, DEBUG_PROPERTY_INFO[] rgelt, out uint pceltFetched)
        {
            pceltFetched = 0;
            return base.Next(celt, rgelt, ref pceltFetched);
        }
    }
}
