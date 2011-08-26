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

        public int GetSourceRange(TEXT_POSITION[] pBegPosition, TEXT_POSITION[] pEndPosition)
        {
            throw new NotImplementedException();
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
