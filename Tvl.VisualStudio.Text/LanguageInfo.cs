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

    public abstract class LanguageInfo : IVsLanguageInfo, IDisposable
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
