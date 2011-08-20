namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class JavaDebugStackFrame
        : IDebugStackFrame3
        , IDebugStackFrame2
        , IDebugQueryEngine2
    {
        #region IDebugStackFrame2 Members

        public int EnumProperties(enum_DEBUGPROP_INFO_FLAGS dwFields, uint nRadix, ref Guid guidFilter, uint dwTimeout, out uint pcelt, out IEnumDebugPropertyInfo2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int GetCodeContext(out IDebugCodeContext2 ppCodeCxt)
        {
            throw new NotImplementedException();
        }

        public int GetDebugProperty(out IDebugProperty2 ppProperty)
        {
            throw new NotImplementedException();
        }

        public int GetDocumentContext(out IDebugDocumentContext2 ppCxt)
        {
            throw new NotImplementedException();
        }

        public int GetExpressionContext(out IDebugExpressionContext2 ppExprCxt)
        {
            throw new NotImplementedException();
        }

        public int GetInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, FRAMEINFO[] pFrameInfo)
        {
            throw new NotImplementedException();
        }

        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            throw new NotImplementedException();
        }

        public int GetName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int GetPhysicalStackRange(out ulong paddrMin, out ulong paddrMax)
        {
            throw new NotImplementedException();
        }

        public int GetThread(out IDebugThread2 ppThread)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugStackFrame3 Members


        public int GetUnwindCodeContext(out IDebugCodeContext2 ppCodeContext)
        {
            throw new NotImplementedException();
        }

        public int InterceptCurrentException(enum_INTERCEPT_EXCEPTION_ACTION dwFlags, out ulong pqwCookie)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugQueryEngine2 Members

        public int GetEngineInterface(out object ppUnk)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
