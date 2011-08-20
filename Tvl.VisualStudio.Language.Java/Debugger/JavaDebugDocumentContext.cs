using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Runtime.InteropServices;

namespace Tvl.VisualStudio.Language.Java.Debugger
{
    [ComVisible(true)]
    public class JavaDebugDocumentContext : IDebugDocumentContext2, IDebugDocumentChecksum2
    {
        #region IDebugDocumentContext2 Members

        public int Compare(enum_DOCCONTEXT_COMPARE Compare, IDebugDocumentContext2[] rgpDocContextSet, uint dwDocContextSetLen, out uint pdwDocContext)
        {
            throw new NotImplementedException();
        }

        public int EnumCodeContexts(out IEnumDebugCodeContexts2 ppEnumCodeCxts)
        {
            throw new NotImplementedException();
        }

        public int GetDocument(out IDebugDocument2 ppDocument)
        {
            throw new NotImplementedException();
        }

        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            throw new NotImplementedException();
        }

        public int GetName(enum_GETNAME_TYPE gnType, out string pbstrFileName)
        {
            throw new NotImplementedException();
        }

        public int GetSourceRange(TEXT_POSITION[] pBegPosition, TEXT_POSITION[] pEndPosition)
        {
            throw new NotImplementedException();
        }

        public int GetStatementRange(TEXT_POSITION[] pBegPosition, TEXT_POSITION[] pEndPosition)
        {
            throw new NotImplementedException();
        }

        public int Seek(int nCount, out IDebugDocumentContext2 ppDocContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugDocumentChecksum2 Members

        public int GetChecksumAndAlgorithmId(out Guid pRetVal, uint cMaxBytes, byte[] pChecksum, out uint pcNumBytes)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
