namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class JavaDebugExpressionContext : IDebugExpressionContext2
    {
        #region IDebugExpressionContext2 Members

        public int GetName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int ParseText(string pszCode, enum_PARSEFLAGS dwFlags, uint nRadix, out IDebugExpression2 ppExpr, out string pbstrError, out uint pichError)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
