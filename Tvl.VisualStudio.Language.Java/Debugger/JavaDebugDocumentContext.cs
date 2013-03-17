namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Runtime.InteropServices;
    using Tvl.Java.DebugInterface;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;

    [ComVisible(true)]
    public class JavaDebugDocumentContext : IDebugDocumentContext2, IDebugDocumentChecksum2
    {
        private readonly ILocation _location;

        public JavaDebugDocumentContext(ILocation location)
        {
            Contract.Requires<ArgumentNullException>(location != null, "location");
            _location = location;
        }

        #region IDebugDocumentContext2 Members

        public int Compare(enum_DOCCONTEXT_COMPARE Compare, IDebugDocumentContext2[] rgpDocContextSet, uint dwDocContextSetLen, out uint pdwDocContext)
        {
            throw new NotImplementedException();
        }

        public int EnumCodeContexts(out IEnumDebugCodeContexts2 ppEnumCodeCxts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the document that contains this document context.
        /// </summary>
        /// <param name="ppDocument">Returns an IDebugDocument2 object that represents the document that contains this document context.</param>
        /// <returns>If successful, returns S_OK; otherwise, returns an error code.</returns>
        /// <remarks>
        /// This method is for those debug engines that supply documents directly to the IDE. Otherwise, this method should return E_NOTIMPL.
        /// </remarks>
        public int GetDocument(out IDebugDocument2 ppDocument)
        {
            // this might be implemented if we support download-on-demand for the jre source in the future
            ppDocument = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            pbstrLanguage = Constants.JavaLanguageName;
            pguidLanguage = Constants.JavaLanguageGuid;
            return VSConstants.S_OK;
        }

        public int GetName(enum_GETNAME_TYPE gnType, out string pbstrFileName)
        {
            pbstrFileName = null;

            switch (gnType)
            {
            case enum_GETNAME_TYPE.GN_BASENAME:
                // Specifies a base file name instead of a full path of the document or context
                pbstrFileName = _location.GetSourceName();
                return VSConstants.S_OK;

            case enum_GETNAME_TYPE.GN_FILENAME:
                // Specifies the full path of the document or context
                pbstrFileName = _location.GetSourcePath();
                return VSConstants.S_OK;

            case enum_GETNAME_TYPE.GN_NAME:
                // Specifies a friendly name of the document or context
                pbstrFileName = _location.GetSourceName();
                return VSConstants.S_OK;

            case enum_GETNAME_TYPE.GN_MONIKERNAME:
                // Specifies a unique name of the document or context in the form of a moniker
                return VSConstants.E_INVALIDARG;

            case enum_GETNAME_TYPE.GN_STARTPAGEURL:
                // Gets the starting page URL for processes
                return VSConstants.E_INVALIDARG;

            case enum_GETNAME_TYPE.GN_TITLE:
                // Specifies a title of the document, if one exists
                return VSConstants.E_INVALIDARG;

            case enum_GETNAME_TYPE.GN_URL:
                // Specifies a URL name of the document or context
                return VSConstants.E_INVALIDARG;

            default:
                return VSConstants.E_INVALIDARG;
            }
        }

        /// <summary>
        /// Gets the source code range of this document context.
        /// </summary>
        /// <param name="pBegPosition">[in, out] A TEXT_POSITION structure that is filled in with the starting position. Set this argument to a null value if this information is not needed.</param>
        /// <param name="pEndPosition">[in, out] A TEXT_POSITION structure that is filled in with the ending position. Set this argument to a null value if this information is not needed.</param>
        /// <returns>If successful, returns S_OK; otherwise, returns an error code.</returns>
        /// <remarks>
        /// A source range is the entire range of source code, from the current statement back to just
        /// after the previous statement that contributed code. The source range is typically used for
        /// mixing source statements, including comments, with code in the disassembly window.
        /// 
        /// To get the range for just the code statements contained within this document context, call
        /// the IDebugDocumentContext2.GetStatementRange method.
        /// </remarks>
        public int GetSourceRange(TEXT_POSITION[] pBegPosition, TEXT_POSITION[] pEndPosition)
        {
            // TODO: also includes lines leading up to this one which do not contain executable code
            if (pBegPosition != null && pBegPosition.Length == 0)
                throw new ArgumentException("pBegPosition");
            if (pEndPosition != null && pEndPosition.Length == 0)
                throw new ArgumentException("pEndPosition");

            TEXT_POSITION begin = new TEXT_POSITION();
            TEXT_POSITION end = new TEXT_POSITION();

            begin.dwLine = (uint)_location.GetLineNumber() - 1;
            begin.dwColumn = 0;
            end = begin;

            if (pBegPosition != null)
                pBegPosition[0] = begin;

            if (pEndPosition != null)
                pEndPosition[0] = end;

            return VSConstants.S_OK;
        }

        public int GetStatementRange(TEXT_POSITION[] pBegPosition, TEXT_POSITION[] pEndPosition)
        {
            pBegPosition[0].dwLine = (uint)_location.GetLineNumber() - 1;
            pBegPosition[0].dwColumn = 0;

            pEndPosition[0].dwLine = (uint)_location.GetLineNumber() - 1;
            pEndPosition[0].dwColumn = 0;

            return VSConstants.S_OK;
        }

        public int Seek(int nCount, out IDebugDocumentContext2 ppDocContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugDocumentChecksum2 Members

        public int GetChecksumAndAlgorithmId(out Guid pRetVal, uint cMaxBytes, byte[] pChecksum, out uint pcNumBytes)
        {
            pRetVal = default(Guid);
            pcNumBytes = default(uint);
            return VSConstants.E_NOTIMPL;
        }

        #endregion
    }
}
