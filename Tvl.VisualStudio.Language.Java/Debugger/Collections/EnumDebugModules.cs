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
    public class EnumDebugModules : DebugEnumerator<IEnumDebugModules2, IDebugModule2>, IEnumDebugModules2
    {
        public EnumDebugModules(IEnumerable<IDebugModule2> modules)
            : base(modules)
        {
            Contract.Requires(modules != null);
        }

        protected EnumDebugModules(IDebugModule2[] elements, int currentIndex)
            : base(elements, currentIndex)
        {
        }

        protected override IEnumDebugModules2 CreateClone(IDebugModule2[] elements, int currentIndex)
        {
            return new EnumDebugModules(elements, currentIndex);
        }
    }
}
