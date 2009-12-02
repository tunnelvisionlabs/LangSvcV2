namespace Tvl.VisualStudio.Shell.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Shell.Interop;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using IOleUndoManager = Microsoft.VisualStudio.OLE.Interop.IOleUndoManager;
    using IOleCommandTarget = Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
    using Microsoft.VisualStudio.Text.Editor;
    using VsShellUtilities = Microsoft.VisualStudio.Shell.VsShellUtilities;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using System.Runtime.InteropServices;
    using Tvl.VisualStudio.Shell.Extensions;

    [Export(typeof(IMonitorSelectionService))]
    internal sealed class MonitorSelectionService : IMonitorSelectionService
    {
        private IServiceProvider _serviceProvider;

        [Import]
        public IServiceProvider GlobalServiceProvider
        {
            get
            {
                return _serviceProvider;
            }
            set
            {
                _serviceProvider = value;
                if (_listener == null)
                    _listener = new Listener(this, _serviceProvider);
            }
        }

        [Import]
        public IVsEditorAdaptersFactoryService VsEditorAdaptorsFactoryService;

        private Listener _listener;

        public event EventHandler<ViewChangedEventArgs> ViewChanged;

        public ITextView CurrentView
        {
            get
            {
                return GetTextView(_listener.CurrentDocumentFrame);
            }
        }

        private ITextView GetTextView(IVsWindowFrame frame)
        {
            if (frame == null)
                return null;

            IVsTextView viewAdapter = VsShellUtilities.GetTextView(frame);
            IWpfTextView wpfTextView = this.VsEditorAdaptorsFactoryService.GetWpfTextView(viewAdapter);
            return wpfTextView;
        }

        private void HandleElementValueChanged(VSConstants.VSSELELEMID elementId, object oldValue, object newValue)
        {
            switch (elementId)
            {
            case VSConstants.VSSELELEMID.SEID_DocumentFrame:
                var viewChanged = ViewChanged;
                if (viewChanged != null)
                {
                    ITextView oldView = GetTextView((IVsWindowFrame)oldValue);
                    ITextView newView = GetTextView((IVsWindowFrame)newValue);
                    ViewChangedEventArgs e = new ViewChangedEventArgs(oldView, newView);
                    viewChanged(this, e);
                }

                return;

            case VSConstants.VSSELELEMID.SEID_LastWindowFrame:
                break;

            case VSConstants.VSSELELEMID.SEID_PropertyBrowserSID:
                break;

            case VSConstants.VSSELELEMID.SEID_ResultList:
                break;

            case VSConstants.VSSELELEMID.SEID_StartupProject:
                break;

            case VSConstants.VSSELELEMID.SEID_UndoManager:
                break;

            case VSConstants.VSSELELEMID.SEID_UserContext:
                break;

            case VSConstants.VSSELELEMID.SEID_WindowFrame:
                break;

            default:
                break;
            }
        }

        [ComVisible(true)]
        private sealed class Listener : IVsSelectionEvents, IDisposable
        {
            private IVsMonitorSelection _monitorSelection;
            private MonitorSelectionService _service;
            private uint _adviseCookie;

            public Listener(MonitorSelectionService service, IServiceProvider serviceProvider)
            {
                Contract.Requires<ArgumentException>(service != null);
                Contract.Requires<ArgumentNullException>(serviceProvider != null);

                IOleServiceProvider olesp = serviceProvider.TryGetOleServiceProvider();
                if (olesp == null)
                    throw new NotSupportedException();

                _service = service;
                _monitorSelection = olesp.TryGetGlobalService<SVsShellMonitorSelection, IVsMonitorSelection>();
                if (_monitorSelection == null)
                    throw new NotSupportedException();

                _monitorSelection.AdviseSelectionEvents(this, out _adviseCookie);
            }

            #region Properties
            public IOleUndoManager CurrentUndoManager
            {
                get
                {
                    return (IOleUndoManager)GetCurrentElementValue(VSConstants.VSSELELEMID.SEID_UndoManager);
                }
            }

            public IVsWindowFrame CurrentWindowFrame
            {
                get
                {
                    return (IVsWindowFrame)GetCurrentElementValue(VSConstants.VSSELELEMID.SEID_WindowFrame);
                }
            }

            public IVsWindowFrame CurrentDocumentFrame
            {
                get
                {
                    return (IVsWindowFrame)GetCurrentElementValue(VSConstants.VSSELELEMID.SEID_DocumentFrame);
                }
            }

            public IVsHierarchy CurrentStartupProject
            {
                get
                {
                    return (IVsHierarchy)GetCurrentElementValue(VSConstants.VSSELELEMID.SEID_StartupProject);
                }
            }

            public IVsPropertyBrowser CurrentPropertyBrowser
            {
                get
                {
                    return (IVsPropertyBrowser)GetCurrentElementValue(VSConstants.VSSELELEMID.SEID_PropertyBrowserSID);
                }
            }

            public IVsUserContext CurrentUserContext
            {
                get
                {
                    return (IVsUserContext)GetCurrentElementValue(VSConstants.VSSELELEMID.SEID_UserContext);
                }
            }

            public IOleCommandTarget CurrentResultList
            {
                get
                {
                    return (IOleCommandTarget)GetCurrentElementValue(VSConstants.VSSELELEMID.SEID_ResultList);
                }
            }

            public IVsWindowFrame LastWindowFrame
            {
                get
                {
                    return (IVsWindowFrame)GetCurrentElementValue(VSConstants.VSSELELEMID.SEID_LastWindowFrame);
                }
            }
            #endregion

            public void Dispose()
            {
                if (_adviseCookie != 0)
                {
                    _monitorSelection.UnadviseSelectionEvents(_adviseCookie);
                    _adviseCookie = 0;
                }

                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Reports that the command UI context has changed.
            /// </summary>
            /// <param name="dwCmdUICookie">DWORD representation of the GUID identifying the command UI context passed in as the rguidCmdUI parameter in the call to GetCmdUIContextCookie.</param>
            /// <param name="fActive">Flag that is set to true if the command UI context identified by dwCmdUICookie has become active and false if it has become inactive.</param>
            /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
            public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
            {
                return VSConstants.S_OK;
            }

            public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
            {
                _service.HandleElementValueChanged((VSConstants.VSSELELEMID)elementid, varValueOld, varValueNew);
                return VSConstants.S_OK;
            }

            /// <summary>
            /// Reports that the project hierarchy, item and/or selection container has changed.
            /// </summary>
            /// <param name="pHierOld">Pointer to the IVsHierarchy interface of the project hierarchy for the previous selection.</param>
            /// <param name="itemidOld">Identifier of the project item for previous selection. For valid itemidOld values, see VSITEMID.</param>
            /// <param name="pMISOld">Pointer to the IVsMultiItemSelect interface to access a previous multiple selection.</param>
            /// <param name="pSCOld">Pointer to the ISelectionContainer interface to access Properties window data for the previous selection.</param>
            /// <param name="pHierNew">Pointer to the IVsHierarchy interface of the project hierarchy for the current selection.</param>
            /// <param name="itemidNew">Identifier of the project item for the current selection. For valid itemidNew values, see VSITEMID.</param>
            /// <param name="pMISNew">Pointer to the IVsMultiItemSelect interface for the current selection.</param>
            /// <param name="pSCNew">Pointer to the ISelectionContainer interface for the current selection.</param>
            /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
            public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
            {
                return VSConstants.S_OK;
            }

            private object GetCurrentElementValue(VSConstants.VSSELELEMID elementId)
            {
                object value = null;
                if (ErrorHandler.Succeeded(ErrorHandler.CallWithCOMConvention(() => _monitorSelection.GetCurrentElementValue((uint)elementId, out value))))
                    return value;

                return null;
            }
        }
    }
}
