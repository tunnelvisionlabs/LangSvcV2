namespace Tvl.VisualStudio.Shell
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
    using Microsoft.VisualStudio.Shell;
    using MemoryStream = System.IO.MemoryStream;
    using Stream = System.IO.Stream;
    using BinaryReader = System.IO.BinaryReader;
    using LARGE_INTEGER = Microsoft.VisualStudio.OLE.Interop.LARGE_INTEGER;
    using ULARGE_INTEGER = Microsoft.VisualStudio.OLE.Interop.ULARGE_INTEGER;
    using STATSTG = Microsoft.VisualStudio.OLE.Interop.STATSTG;

    public abstract class ToolWindow : IToolWindow, IVsUIElementPane, IDisposable
    {
        private HwndSource _source;

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

        public bool MultiInstance
        {
            get
            {
                return false;
            }
        }

        public ToolWindowOrientation Orientation
        {
            get
            {
                return ToolWindowOrientation.Left;
            }
        }

        public VsDockStyle Style
        {
            get
            {
                return VsDockStyle.Linked;
            }
        }

        public bool Transient
        {
            get
            {
                return true;
            }
        }

        protected abstract FrameworkElement CreateVisualElement();

        #region IVsUIElementPane Members

        int IVsUIElementPane.CloseUIElementPane()
        {
            this.OnClose();
            return VSConstants.S_OK;
        }

        int IVsUIElementPane.CreateUIElementPane(out object punkUIElement)
        {
            VisualElement = CreateVisualElement();
            punkUIElement = VisualElement;
            //uiElement = null;
            //if (this.InitializationMode != PaneInitializationMode.Uninitialized)
            //{
            //    throw new InvalidOperationException("The WindowPane is already initialized");
            //}
            //this.InitializationMode = PaneInitializationMode.IVsUIElementPane;
            //this.OnCreate();
            //if (this.Content != null)
            //{
            //    uiElement = this.Content;
            //}
            //else if (this.Window != null)
            //{
            //    this.win32Wrapper = new UIWin32ElementWrapper(this);
            //    uiElement = this.win32Wrapper;
            //}
            //else
            //{
            //    return -2147418113;
            //}
            return 0;
        }

        int IVsUIElementPane.GetDefaultUIElementSize(SIZE[] psize)
        {
            return VSConstants.E_NOTIMPL;
        }

        int IVsUIElementPane.LoadUIElementState(IStream pstream)
        {
            byte[] bufferFromIStream = GetBufferFromIStream(pstream);
            if (bufferFromIStream.Length > 0)
            {
                using (MemoryStream stream = new MemoryStream(bufferFromIStream))
                {
                    return this.LoadUIState(stream);
                }
            }
            return 0;
        }

        int IVsUIElementPane.SaveUIElementState(IStream pstream)
        {
            Stream stream;
            int hr = this.SaveUIState(out stream);
            if (!ErrorHandler.Succeeded(hr))
            {
                return hr;
            }
            using (stream)
            {
                if (((stream == null) || !stream.CanRead) || (stream.Length <= 0L))
                {
                    return hr;
                }
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Position = 0L;
                    reader.Read(buffer, 0, buffer.Length);
                    uint pcbWritten = 0;
                    pstream.Write(buffer, (uint)buffer.Length, out pcbWritten);
                    pstream.Commit(0);
                    return hr;
                }
            }
        }

        int IVsUIElementPane.SetUIElementSite(IOleServiceProvider pSP)
        {
            ServiceProvider = pSP;
            return VSConstants.S_OK;
        }

        int IVsUIElementPane.TranslateUIElementAccelerator(MSG[] lpmsg)
        {
            return this.InternalTranslateAccelerator(lpmsg);
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HwndSource source = _source;
                if (source != null)
                {
                    source.Dispose();
                    _source = null;
                }
            }
        }

        protected virtual void OnClose()
        {
            this.Dispose();
        }

        public virtual int LoadUIState(Stream stateStream)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int SaveUIState(out Stream stateStream)
        {
            stateStream = null;
            return VSConstants.S_OK;
        }

        //protected virtual bool PreProcessMessage(ref Message m)
        //{
        //    IntPtr hwnd;
        //    var source = HwndSource.FromHwnd(hwnd);
        //    var visual = source.RootVisual;
        //    visual.Dispatcher.
        //    Control control = Control.FromChildHandle(m.HWnd);
        //    return ((control != null) && (control.PreProcessControlMessage(ref m) == PreProcessControlState.MessageProcessed));
        //}

        private int InternalTranslateAccelerator(MSG[] msg)
        {
            //Message m = Message.Create(msg[0].hwnd, (int)msg[0].message, msg[0].wParam, msg[0].lParam);
            //bool flag = this.PreProcessMessage(ref m);
            //msg[0].message = (uint)m.Msg;
            //msg[0].wParam = m.WParam;
            //msg[0].lParam = m.LParam;
            //if (flag)
            //{
            //    return VSConstants.S_OK;
            //}
            return VSConstants.E_FAIL;
        }

        private static byte[] GetBufferFromIStream(IStream comStream)
        {
            LARGE_INTEGER large_integer;
            LARGE_INTEGER large_integer2;
            large_integer.QuadPart = 0L;
            ULARGE_INTEGER[] plibNewPosition = new ULARGE_INTEGER[1];
            comStream.Seek(large_integer, 1, plibNewPosition);
            comStream.Seek(large_integer, 0, null);
            STATSTG[] pstatstg = new STATSTG[1];
            comStream.Stat(pstatstg, 1);
            int quadPart = (int)pstatstg[0].cbSize.QuadPart;
            byte[] pv = new byte[quadPart];
            uint pcbRead = 0;
            comStream.Read(pv, (uint)pv.Length, out pcbRead);
            large_integer2.QuadPart = (long)plibNewPosition[0].QuadPart;
            comStream.Seek(large_integer2, 0, null);
            return pv;
        }
    }
}
