namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class JavaDebugFunctionPosition : IDebugFunctionPosition2
    {
        #region IDebugFunctionPosition2 Members

        public int GetFunctionName(out string pbstrFunctionName)
        {
            throw new NotImplementedException();
        }

        public int GetOffset(TEXT_POSITION[] pPosition)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
