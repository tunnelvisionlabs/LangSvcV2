namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell.Extensions;

    [Guid(AlloyConstants.AlloyLanguageGuidString)]
    internal class AlloyLanguageInfo : IVsLanguageInfo
    {
        private readonly SVsServiceProvider _serviceProvider;

        public AlloyLanguageInfo(SVsServiceProvider serviceProvider)
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

            ppCodeWinMgr = textBuffer.Properties.GetOrCreateSingletonProperty<CodeWindowManager>(() => new CodeWindowManager());
            return VSConstants.S_OK;
        }

        int IVsLanguageInfo.GetColorizer(IVsTextLines pBuffer, out IVsColorizer ppColorizer)
        {
            ppColorizer = null;
            return VSConstants.E_FAIL;
        }

        int IVsLanguageInfo.GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = AlloyConstants.AlloyFileExtension;
            return VSConstants.S_OK;
        }

        int IVsLanguageInfo.GetLanguageName(out string bstrName)
        {
            bstrName = AlloyConstants.AlloyLanguageName;
            return VSConstants.S_OK;
        }
    }
}
