namespace Tvl.VisualStudio.Text
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell.Extensions;

    public abstract class LanguageInfo : IVsLanguageInfo
    {
        private readonly SVsServiceProvider _serviceProvider;

        public LanguageInfo(SVsServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _serviceProvider = serviceProvider;
        }

        public IComponentModel ComponentModel
        {
            get
            {
                return _serviceProvider.GetComponentModel();
            }
        }

        public SVsServiceProvider ServiceProvider
        {
            get
            {
                return _serviceProvider;
            }
        }

        public abstract LanguagePreferences LanguagePreferences
        {
            get;
        }

        public abstract string LanguageName
        {
            get;
        }

        public virtual int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr)
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

            ppCodeWinMgr = textBuffer.Properties.GetOrCreateSingletonProperty<CodeWindowManager>(() => new CodeWindowManager(pCodeWin, ServiceProvider, LanguagePreferences));
            return VSConstants.S_OK;
        }

        public virtual int GetColorizer(IVsTextLines pBuffer, out IVsColorizer ppColorizer)
        {
            ppColorizer = null;
            return VSConstants.E_FAIL;
        }

        public virtual int GetLanguageName(out string bstrName)
        {
            bstrName = LanguageName;
            return string.IsNullOrEmpty(bstrName) ? VSConstants.E_FAIL : VSConstants.S_OK;
        }

        public abstract int GetFileExtensions(out string pbstrExtensions);
    }
}
