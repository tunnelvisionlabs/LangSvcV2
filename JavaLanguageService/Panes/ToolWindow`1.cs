namespace JavaLanguageService.Panes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Media;
    using System.Windows;
    using Microsoft.VisualStudio.Shell.Interop;
    using SIZE = Microsoft.VisualStudio.OLE.Interop.SIZE;
    using IStream = Microsoft.VisualStudio.OLE.Interop.IStream;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    using MSG = Microsoft.VisualStudio.OLE.Interop.MSG;
    using Microsoft.VisualStudio;
    using System.Windows.Controls;
    using System.Windows.Interop;

    public class ToolWindow<T> : IToolWindow, IVsWindowPane
    //where T : FrameworkElement, new
    {
        public ToolWindow(string caption, ImageSource icon)
        {
            this.Caption = caption;
            this.Icon = icon;
        }

        public string Caption
        {
            get;
            private set;
        }

        public ImageSource Icon
        {
            get;
            private set;
        }

        public FrameworkElement VisualElement
        {
            get;
            private set;
        }

        private IOleServiceProvider ServiceProvider
        {
            get;
            set;
        }

        #region IVsWindowPane Members

        int IVsWindowPane.ClosePane()
        {
            return VSConstants.S_OK;
        }

        int IVsWindowPane.CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr hwnd)
        {
            HwndSource parent = HwndSource.FromHwnd(hwndParent);
            parent.RootVisual = VisualElement;
            hwnd = IntPtr.Zero;
            return VSConstants.S_OK;
        }

        int IVsWindowPane.GetDefaultSize(SIZE[] pSize)
        {
            pSize[0].cx = (int)VisualElement.DesiredSize.Width;
            pSize[0].cy = (int)VisualElement.DesiredSize.Height;
            return VSConstants.S_OK;
        }

        int IVsWindowPane.LoadViewState(IStream pStream)
        {
            return VSConstants.S_OK;
        }

        int IVsWindowPane.SaveViewState(IStream pStream)
        {
            return VSConstants.S_OK;
        }

        int IVsWindowPane.SetSite(IOleServiceProvider psp)
        {
            ServiceProvider = psp;
            return VSConstants.S_OK;
        }

        int IVsWindowPane.TranslateAccelerator(MSG[] lpmsg)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion
    }
}
