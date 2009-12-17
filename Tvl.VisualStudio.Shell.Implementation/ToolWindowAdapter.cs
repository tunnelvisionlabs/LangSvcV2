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
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    internal sealed class ToolWindowAdapter : IVsUIElementPane, IDisposable
    {
        private HwndSource _source;
        private IToolWindow _content;
        private FrameworkElement _visualElement;

        public ToolWindowAdapter(IToolWindow content)
        {
            this._content = content;
        }

        private IOleServiceProvider ServiceProvider
        {
            get;
            set;
        }

        #region IVsUIElementPane Members

        int IVsUIElementPane.CloseUIElementPane()
        {
            this.OnClose();
            return VSConstants.S_OK;
        }

        int IVsUIElementPane.CreateUIElementPane(out object punkUIElement)
        {
            _visualElement = _content.CreateContent();
            punkUIElement = _visualElement;
            return VSConstants.S_OK;
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
            return VSConstants.S_OK;
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

        private void Dispose(bool disposing)
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

        private void OnClose()
        {
            this.Dispose();
        }

        private int LoadUIState(Stream stateStream)
        {
            return VSConstants.E_NOTIMPL;
        }

        private int SaveUIState(out Stream stateStream)
        {
            stateStream = null;
            return VSConstants.S_OK;
        }

        private int InternalTranslateAccelerator(MSG[] lpmsg)
        {
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
