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
    public class EnumDebugReferenceInfo : DebugEnumerator<IEnumDebugReferenceInfo2, DEBUG_REFERENCE_INFO>, IEnumDebugReferenceInfo2
    {
        public EnumDebugReferenceInfo(IEnumerable<DEBUG_REFERENCE_INFO> referenceInfo)
            :  base(referenceInfo)
        {
            Contract.Requires(referenceInfo != null);
        }

        int IEnumDebugReferenceInfo2.Next(uint celt, DEBUG_REFERENCE_INFO[] rgelt, out uint pceltFetched)
        {
            pceltFetched = 0;
            return base.Next(celt, rgelt, ref pceltFetched);
        }
    }
}
