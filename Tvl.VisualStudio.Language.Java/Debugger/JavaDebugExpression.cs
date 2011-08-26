namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;
    using Antlr.Runtime.Tree;
    using System.Diagnostics.Contracts;

    [ComVisible(true)]
    public class JavaDebugExpression : IDebugExpression2
    {
        private readonly JavaDebugExpressionContext _context;
        private readonly CommonTree _expression;

        public JavaDebugExpression(JavaDebugExpressionContext context, CommonTree expression)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(expression != null, "expression");

            _context = context;
            _expression = expression;
        }

        #region IDebugExpression2 Members

        public int Abort()
        {
            throw new NotImplementedException();
        }

        public int EvaluateAsync(enum_EVALFLAGS dwFlags, IDebugEventCallback2 pExprCallback)
        {
            throw new NotImplementedException();
        }

        public int EvaluateSync(enum_EVALFLAGS dwFlags, uint dwTimeout, IDebugEventCallback2 pExprCallback, out IDebugProperty2 ppResult)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
