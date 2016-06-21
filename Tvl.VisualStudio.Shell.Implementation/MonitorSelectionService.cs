namespace Tvl.VisualStudio.Shell.Implementation
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using IOleCommandTarget = Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
    using IOleUndoManager = Microsoft.VisualStudio.OLE.Interop.IOleUndoManager;
    using VsShellUtilities = Microsoft.VisualStudio.Shell.VsShellUtilities;

    [Export(typeof(IMonitorSelectionService))]
    internal sealed class MonitorSelectionService : IMonitorSelectionService, IPartImportsSatisfiedNotification
    {
        private Listener _listener;

        public event EventHandler<ViewChangedEventArgs> ViewChanged;

        [Import]
        private SVsServiceProvider GlobalServiceProvider
        {
            get;
            set;
        }

        [Import]
        private IVsEditorAdaptersFactoryService VsEditorAdaptorsFactoryService
        {
            get;
            set;
        }

        public ITextView CurrentView
        {
            get
            {
                return GetTextView(_listener.CurrentDocumentFrame);
            }
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            _listener = new Listener(this, GlobalServiceProvider);
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

            public Listener(MonitorSelectionService service, SVsServiceProvider serviceProvider)
            {
                Contract.Requires<ArgumentNullException>(service != null, "service");
                Contract.Requires<ArgumentNullException>(serviceProvider != null, "serviceProvider");

                _service = service;
                _monitorSelection = serviceProvider.GetMonitorSelection();
                _monitorSelection.AdviseSelectionEvents(this, out _adviseCookie);
            }

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
