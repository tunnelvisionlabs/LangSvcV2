namespace JavaLanguageService.Text
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.TextManager.Interop;

    using IOleCommandTarget = Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
    using OLECMD = Microsoft.VisualStudio.OLE.Interop.OLECMD;
    using OLECMDEXECOPT = Microsoft.VisualStudio.OLE.Interop.OLECMDEXECOPT;
    using OLECMDF = Microsoft.VisualStudio.OLE.Interop.OLECMDF;
    using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
    using VsMenus = Microsoft.VisualStudio.Shell.VsMenus;

    [ComVisible(true)]
    public abstract class TextViewCommandFilter : IOleCommandTarget, IDisposable
    {
        private bool _hooked;
        private IOleCommandTarget _next;

        protected TextViewCommandFilter(IVsTextView textViewAdapter)
        {
            this.TextViewAdapter = textViewAdapter;
        }

        protected IVsTextView TextViewAdapter
        {
            get;
            private set;
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return _hooked;
            }
            set
            {
                ThrowIfDisposed();
                if (_hooked == value)
                    return;

                if (value)
                {
                    IOleCommandTarget next;
                    ErrorHandler.ThrowOnFailure(TextViewAdapter.AddCommandFilter(this, out next));
                    _next = next;
                }
                else
                {
                    int hr = TextViewAdapter.RemoveCommandFilter(this);
                    if (!IsDisposing)
                        ErrorHandler.ThrowOnFailure(hr);

                    _next = null;
                }

                _hooked = value;
            }
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public bool IsDisposing
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (IsDisposing)
                throw new InvalidOperationException();

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            IsDisposing = true;
            try
            {
                if (!IsDisposed)
                {
                    Enabled = false;
                }
            }
            finally
            {
                IsDisposed = true;
                IsDisposing = false;
            }
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private int ExecCommand(ref Guid guidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            int rc = 0;

            if (!HandlePreExec(ref guidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut) && _next != null)
            {
                // Pass it along the chain.
                rc = this.InnerExec(ref guidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
                if (!ErrorHandler.Succeeded(rc))
                    return rc;

                HandlePostExec(ref guidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }

            return rc;
        }

        protected virtual bool HandlePreExec(ref Guid guidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return false;
        }

        private int InnerExec(ref Guid guidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return _next.Exec(ref guidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        protected virtual void HandlePostExec(ref Guid guidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
        }

        protected virtual int QueryParameterList(ref Guid guidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        protected virtual OLECMDF QueryCommandStatus(ref Guid guidCmdGroup, uint cmdId)
        {
            return 0;
        }

        int IOleCommandTarget.Exec(ref Guid guidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            ushort lo = (ushort)(nCmdexecopt & (uint)0xffff);
            ushort hi = (ushort)(nCmdexecopt >> 16);

            switch (lo)
            {
            case (ushort)OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP:
                if ((nCmdexecopt >> 16) == VsMenus.VSCmdOptQueryParameterList)
                {
                    return QueryParameterList(ref guidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
                }
                break;

            default:
                return ExecCommand(ref guidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }

            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        int IOleCommandTarget.QueryStatus(ref Guid guidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            Guid cmdGroup = guidCmdGroup;
            for (uint i = 0; i < cCmds; i++)
            {
                OLECMDF status = QueryCommandStatus(ref cmdGroup, prgCmds[i].cmdID);
                if (status == 0 && _next != null)
                {
                    return _next.QueryStatus(ref cmdGroup, cCmds, prgCmds, pCmdText);
                }

                prgCmds[i].cmdf = (uint)status;
            }

            return VSConstants.S_OK;
        }
    }
}
