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
    public class EnumDebugFrameInfo : DebugEnumerator<IEnumDebugFrameInfo2, FRAMEINFO>, IEnumDebugFrameInfo2
    {
        public EnumDebugFrameInfo(IEnumerable<FRAMEINFO> frameInfo)
            : base(frameInfo)
        {
            Contract.Requires(frameInfo != null);
        }

        protected EnumDebugFrameInfo(FRAMEINFO[] elements, int currentIndex)
            : base(elements, currentIndex)
        {
        }

        protected override IEnumDebugFrameInfo2 CreateClone(FRAMEINFO[] elements, int currentIndex)
        {
            return new EnumDebugFrameInfo(elements, currentIndex);
        }
    }
}
