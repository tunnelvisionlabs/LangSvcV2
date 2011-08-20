namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell.Extensions;
    using IConnectionPointContainer = Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer;
    using System.Diagnostics.Contracts;

    public abstract class LanguageInfo : IVsLanguageInfo, IVsLanguageDebugInfo, IDisposable
    {
        private readonly SVsServiceProvider _serviceProvider;
        private readonly Guid _languageGuid;
        private LanguagePreferences _languagePreferences;
        private IDisposable _languagePreferencesCookie;

        public LanguageInfo(SVsServiceProvider serviceProvider, Guid languageGuid)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _serviceProvider = serviceProvider;
            _languageGuid = languageGuid;

            IVsTextManager2 textManager = serviceProvider.GetTextManager2();
            LANGPREFERENCES2[] preferences = new LANGPREFERENCES2[1];
            preferences[0].guidLang = languageGuid;
            ErrorHandler.ThrowOnFailure(textManager.GetUserPreferences2(null, null, preferences, null));
            _languagePreferences = CreateLanguagePreferences(preferences[0]);
            _languagePreferencesCookie = ((IConnectionPointContainer)textManager).Advise<LanguagePreferences, IVsTextManagerEvents2>(_languagePreferences);
        }

        public abstract string LanguageName
        {
            get;
        }

        public abstract IEnumerable<string> FileExtensions
        {
            get;
        }

        public LanguagePreferences LanguagePreferences
        {
            get
            {
                return _languagePreferences;
            }
        }

        public Guid LanguageGuid
        {
            get
            {
                return _languageGuid;
            }
        }

        protected SVsServiceProvider ServiceProvider
        {
            get
            {
                return _serviceProvider;
            }
        }

        protected IComponentModel ComponentModel
        {
            get
            {
                return _serviceProvider.GetComponentModel();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual int GetColorizer(IVsTextLines buffer, out IVsColorizer colorizer)
        {
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");

            colorizer = null;
            return VSConstants.E_FAIL;
        }

        public virtual int GetLanguageID(IVsTextBuffer buffer, int line, int col, out Guid languageId)
        {
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");

            languageId = LanguageGuid;
            return VSConstants.S_OK;
        }

        [Obsolete]
        public virtual int GetLocationOfName(string name, out string pbstrMkDoc, TextSpan[] spans)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));

            pbstrMkDoc = null;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int GetNameOfLocation(IVsTextBuffer buffer, int line, int col, out string name, out int lineOffset)
        {
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");

            name = null;
            lineOffset = 0;
            return VSConstants.S_OK;
        }

        public virtual int GetProximityExpressions(IVsTextBuffer buffer, int line, int col, int cLines, out IVsEnumBSTR expressions)
        {
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");

            expressions = null;
            return VSConstants.S_FALSE;
        }

        public virtual int IsMappedLocation(IVsTextBuffer buffer, int line, int col)
        {
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");
            return VSConstants.S_FALSE;
        }

        public virtual int ResolveName(string name, RESOLVENAMEFLAGS flags, out IVsEnumDebugName names)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));

            names = null;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int ValidateBreakpointLocation(IVsTextBuffer buffer, int line, int col, TextSpan[] pCodeSpan)
        {
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");
            Contract.Requires<ArgumentNullException>(pCodeSpan != null, "pCodeSpan");
            Contract.Requires<ArgumentException>(pCodeSpan.Length > 0);

            return VSConstants.E_NOTIMPL;
        }

        int IVsLanguageInfo.GetColorizer(IVsTextLines buffer, out IVsColorizer colorizer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return GetColorizer(buffer, out colorizer);
        }

        int IVsLanguageInfo.GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr)
        {
            IVsEditorAdaptersFactoryService adaptersFactory = ComponentModel.GetService<IVsEditorAdaptersFactoryService>();

            IVsTextLines textLines;
            ErrorHandler.ThrowOnFailure(pCodeWin.GetBuffer(out textLines));
            IVsTextBuffer bufferAdapter = (IVsTextBuffer)textLines;
            ITextBuffer textBuffer = adaptersFactory.GetDataBuffer(bufferAdapter);
            if (textBuffer == null)
            {
                ppCodeWinMgr = null;
                return VSConstants.E_FAIL;
            }

            ppCodeWinMgr = GetCodeWindowManager(pCodeWin, textBuffer);
            return VSConstants.S_OK;
        }

        int IVsLanguageInfo.GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = string.Join(";", FileExtensions);
            return VSConstants.S_OK;
        }

        int IVsLanguageInfo.GetLanguageName(out string bstrName)
        {
            bstrName = LanguageName;
            return string.IsNullOrEmpty(bstrName) ? VSConstants.E_FAIL : VSConstants.S_OK;
        }

        int IVsLanguageDebugInfo.GetLanguageID(IVsTextBuffer pBuffer, int iLine, int iCol, out Guid pguidLanguageID)
        {
            if (pBuffer == null)
                throw new ArgumentNullException("pBuffer");

            return GetLanguageID(pBuffer, iLine, iCol, out pguidLanguageID);
        }

        [Obsolete]
        int IVsLanguageDebugInfo.GetLocationOfName(string pszName, out string pbstrMkDoc, TextSpan[] pspanLocation)
        {
            if (pszName == null)
                throw new ArgumentNullException("pszName");
            if (pszName.Length == 0)
                throw new ArgumentException();

            return GetLocationOfName(pszName, out pbstrMkDoc, pspanLocation);
        }

        int IVsLanguageDebugInfo.GetNameOfLocation(IVsTextBuffer pBuffer, int iLine, int iCol, out string pbstrName, out int piLineOffset)
        {
            if (pBuffer == null)
                throw new ArgumentNullException("pBuffer");

            return GetNameOfLocation(pBuffer, iLine, iCol, out pbstrName, out piLineOffset);
        }

        int IVsLanguageDebugInfo.GetProximityExpressions(IVsTextBuffer pBuffer, int iLine, int iCol, int cLines, out IVsEnumBSTR ppEnum)
        {
            if (pBuffer == null)
                throw new ArgumentNullException("pBuffer");

            return GetProximityExpressions(pBuffer, iLine, iCol, cLines, out ppEnum);
        }

        int IVsLanguageDebugInfo.IsMappedLocation(IVsTextBuffer pBuffer, int iLine, int iCol)
        {
            if (pBuffer == null)
                throw new ArgumentNullException("pBuffer");

            return IsMappedLocation(pBuffer, iLine, iCol);
        }

        int IVsLanguageDebugInfo.ResolveName(string pszName, uint dwFlags, out IVsEnumDebugName ppNames)
        {
            if (pszName == null)
                throw new ArgumentNullException("pszName");
            if (pszName.Length == 0)
                throw new ArgumentException();

            return ResolveName(pszName, (RESOLVENAMEFLAGS)dwFlags, out ppNames);
        }

        int IVsLanguageDebugInfo.ValidateBreakpointLocation(IVsTextBuffer pBuffer, int iLine, int iCol, TextSpan[] pCodeSpan)
        {
            if (pBuffer == null)
                throw new ArgumentNullException("pBuffer");

            return ValidateBreakpointLocation(pBuffer, iLine, iCol, pCodeSpan);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_languagePreferencesCookie != null)
                {
                    _languagePreferencesCookie.Dispose();
                    _languagePreferencesCookie = null;
                }
            }
        }

        protected virtual LanguagePreferences CreateLanguagePreferences(LANGPREFERENCES2 preferences)
        {
            Contract.Ensures(Contract.Result<LanguagePreferences>() != null);

            return new LanguagePreferences(preferences);
        }

        protected virtual IVsCodeWindowManager GetCodeWindowManager(IVsCodeWindow codeWindow, ITextBuffer textBuffer)
        {
            Contract.Requires<ArgumentNullException>(codeWindow != null, "codeWindow");
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");
            Contract.Ensures(Contract.Result<IVsCodeWindowManager>() != null);

            return textBuffer.Properties.GetOrCreateSingletonProperty<CodeWindowManager>(() => new CodeWindowManager(codeWindow, ServiceProvider, LanguagePreferences));
        }
    }
}
