namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Shell.Interop;
    using Tvl.VisualStudio.Shell;
    using ServiceProvider = Microsoft.VisualStudio.Shell.ServiceProvider;
    using VSConstants = Microsoft.VisualStudio.VSConstants;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    using Marshal = System.Runtime.InteropServices.Marshal;
    using IObjectWithSite = Microsoft.VisualStudio.OLE.Interop.IObjectWithSite;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using IVsCodeWindow = Microsoft.VisualStudio.TextManager.Interop.IVsCodeWindow;
    using IComponentModel = Microsoft.VisualStudio.ComponentModelHost.IComponentModel;
    using SComponentModel = Microsoft.VisualStudio.ComponentModelHost.SComponentModel;
    using READONLYSTATUS = Microsoft.VisualStudio.TextManager.Interop.READONLYSTATUS;
    using IVsUserData = Microsoft.VisualStudio.TextManager.Interop.IVsUserData;
    using IVsTextManager = Microsoft.VisualStudio.TextManager.Interop.IVsTextManager;
    using VsTextBufferClass = Microsoft.VisualStudio.TextManager.Interop.VsTextBufferClass;
    using VsCodeWindowClass = Microsoft.VisualStudio.TextManager.Interop.VsCodeWindowClass;
    using IVsEditorAdaptersFactoryService = Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService;
    using SVsTextManager = Microsoft.VisualStudio.TextManager.Interop.SVsTextManager;
    using IConnectionPoint = Microsoft.VisualStudio.OLE.Interop.IConnectionPoint;
    using IConnectionPointContainer = Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;
    using IVsTextLines = Microsoft.VisualStudio.TextManager.Interop.IVsTextLines;
    using IVsTextBufferDataEvents = Microsoft.VisualStudio.TextManager.Interop.IVsTextBufferDataEvents;
    using Microsoft.VisualStudio.Text.Projection;
    using Microsoft.VisualStudio.Utilities;
    using IVsTextView = Microsoft.VisualStudio.TextManager.Interop.IVsTextView;
    using Microsoft.VisualStudio.Text;
    using System.Runtime.InteropServices;
    using Path = System.IO.Path;

    [Guid("F25F5E78-96E7-4B63-9782-A81F7B0C3DE5")]
    public class PhpEditorFactory : IVsEditorFactory
    {
        private PhpLanguagePackage _package;
        private ServiceProvider _serviceProvider;
        private readonly bool _promptEncodingOnLoad;

        public PhpEditorFactory(PhpLanguagePackage package)
        {
            _package = package;
        }

        public PhpEditorFactory(PhpLanguagePackage package, bool promptEncodingOnLoad)
        {
            _package = package;
            _promptEncodingOnLoad = promptEncodingOnLoad;
        }

        #region IVsEditorFactory Members

        public virtual int SetSite(IOleServiceProvider psp)
        {
            _serviceProvider = new ServiceProvider(psp);
            return VSConstants.S_OK;
        }

        public virtual object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        // This method is called by the Environment (inside IVsUIShellOpenDocument::
        // OpenStandardEditor and OpenSpecificEditor) to map a LOGICAL view to a 
        // PHYSICAL view. A LOGICAL view identifies the purpose of the view that is
        // desired (e.g. a view appropriate for Debugging [LOGVIEWID_Debugging], or a 
        // view appropriate for text view manipulation as by navigating to a find
        // result [LOGVIEWID_TextView]). A PHYSICAL view identifies an actual type 
        // of view implementation that an IVsEditorFactory can create. 
        //
        // NOTE: Physical views are identified by a string of your choice with the 
        // one constraint that the default/primary physical view for an editor  
        // *MUST* use a NULL string as its physical view name (*pbstrPhysicalView = NULL).
        //
        // NOTE: It is essential that the implementation of MapLogicalView properly
        // validates that the LogicalView desired is actually supported by the editor.
        // If an unsupported LogicalView is requested then E_NOTIMPL must be returned.
        //
        // NOTE: The special Logical Views supported by an Editor Factory must also 
        // be registered in the local registry hive. LOGVIEWID_Primary is implicitly 
        // supported by all editor types and does not need to be registered.
        // For example, an editor that supports a ViewCode/ViewDesigner scenario
        // might register something like the following:
        //        HKLM\Software\Microsoft\VisualStudio\9.0\Editors\
        //            {...guidEditor...}\
        //                LogicalViews\
        //                    {...LOGVIEWID_TextView...} = s ''
        //                    {...LOGVIEWID_Code...} = s ''
        //                    {...LOGVIEWID_Debugging...} = s ''
        //                    {...LOGVIEWID_Designer...} = s 'Form'
        //
        public virtual int MapLogicalView(ref Guid logicalView, out string physicalView)
        {
            // initialize out parameter
            physicalView = null;

            bool isSupportedView = false;
            // Determine the physical view
            if (VSConstants.LOGVIEWID_Primary == logicalView ||
                VSConstants.LOGVIEWID_Debugging == logicalView ||
                VSConstants.LOGVIEWID_Code == logicalView ||
                VSConstants.LOGVIEWID_TextView == logicalView)
            {
                // primary view uses NULL as pbstrPhysicalView
                isSupportedView = true;
            }
            else if (VSConstants.LOGVIEWID_Designer == logicalView)
            {
                physicalView = "Design";
                isSupportedView = true;
            }

            if (isSupportedView)
                return VSConstants.S_OK;
            else
            {
                // E_NOTIMPL must be returned for any unrecognized rguidLogicalView values
                return VSConstants.E_NOTIMPL;
            }
        }

        public virtual int Close()
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grfCreateDoc"></param>
        /// <param name="pszMkDocument"></param>
        /// <param name="pszPhysicalView"></param>
        /// <param name="pvHier"></param>
        /// <param name="itemid"></param>
        /// <param name="punkDocDataExisting"></param>
        /// <param name="ppunkDocView"></param>
        /// <param name="ppunkDocData"></param>
        /// <param name="pbstrEditorCaption"></param>
        /// <param name="pguidCmdUI"></param>
        /// <param name="pgrfCDW"></param>
        /// <returns></returns>
        public virtual int CreateEditorInstance(
                        uint createEditorFlags,
                        string documentMoniker,
                        string physicalView,
                        IVsHierarchy hierarchy,
                        uint itemid,
                        System.IntPtr docDataExisting,
                        out System.IntPtr docView,
                        out System.IntPtr docData,
                        out string editorCaption,
                        out Guid commandUIGuid,
                        out int createDocumentWindowFlags)
        {
            // Initialize output parameters
            docView = IntPtr.Zero;
            docData = IntPtr.Zero;
            commandUIGuid = this.GetType().GUID;
            createDocumentWindowFlags = 0;
            editorCaption = null;

            // Validate inputs
            if ((createEditorFlags & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
            {
                return VSConstants.E_INVALIDARG;
            }

            // Get a text buffer
            IVsTextLines textLines = GetTextBuffer(docDataExisting, documentMoniker);

            // Assign docData IntPtr to either existing docData or the new text buffer
            if (docDataExisting != IntPtr.Zero)
            {
                docData = docDataExisting;
                Marshal.AddRef(docData);
            }
            else
            {
                docData = Marshal.GetIUnknownForObject(textLines);
            }

            try
            {
                docView = CreateDocumentView(documentMoniker, physicalView, hierarchy, itemid, textLines, docDataExisting == IntPtr.Zero, out editorCaption, out commandUIGuid);
            }
            finally
            {
                if (docView == IntPtr.Zero)
                {
                    if (docDataExisting != docData && docData != IntPtr.Zero)
                    {
                        // Cleanup the instance of the docData that we have addref'ed
                        Marshal.Release(docData);
                        docData = IntPtr.Zero;
                    }
                }
            }
            return VSConstants.S_OK;
        }


        #endregion

        #region Helper methods

        private IVsTextLines GetTextBuffer(System.IntPtr docDataExisting, string filename)
        {
            IVsTextLines textLines;
            if (docDataExisting == IntPtr.Zero)
            {
                // Create a new IVsTextLines buffer.
                Type textLinesType = typeof(IVsTextLines);
                Guid riid = textLinesType.GUID;
                Guid clsid = typeof(VsTextBufferClass).GUID;
                textLines = _package.CreateInstance(ref clsid, ref riid, textLinesType) as IVsTextLines;

                // set the buffer's site
                ((IObjectWithSite)textLines).SetSite(_serviceProvider.GetService(typeof(IOleServiceProvider)));
            }
            else
            {
                // Use the existing text buffer
                Object dataObject = Marshal.GetObjectForIUnknown(docDataExisting);
                textLines = dataObject as IVsTextLines;
                if (textLines == null)
                {
                    // Try get the text buffer from textbuffer provider
                    IVsTextBufferProvider textBufferProvider = dataObject as IVsTextBufferProvider;
                    if (textBufferProvider != null)
                    {
                        textBufferProvider.GetTextBuffer(out textLines);
                    }
                }
                if (textLines == null)
                {
                    // Unknown docData type then, so we have to force VS to close the other editor.
                    ErrorHandler.ThrowOnFailure((int)VSConstants.VS_E_INCOMPATIBLEDOCDATA);
                }

            }
            return textLines;
        }

        private IntPtr CreateDocumentView(string documentMoniker, string physicalView, IVsHierarchy hierarchy, uint itemid, IVsTextLines textLines, bool createdDocData, out string editorCaption, out Guid cmdUI)
        {
            //Init out params
            editorCaption = string.Empty;
            cmdUI = Guid.Empty;

            if (string.IsNullOrEmpty(physicalView))
            {
                // create code window as default physical view
                return CreateCodeView(documentMoniker, textLines, createdDocData, ref editorCaption, ref cmdUI);
            }

            // We couldn't create the view
            // Return special error code so VS can try another editor factory.
            ErrorHandler.ThrowOnFailure((int)VSConstants.VS_E_UNSUPPORTEDFORMAT);

            return IntPtr.Zero;
        }

        private IntPtr CreateCodeView(string documentMoniker, IVsTextLines textLines, bool createdDocData, ref string editorCaption, ref Guid cmdUI)
        {
            Type codeWindowType = typeof(IVsCodeWindow);
            Guid riid = codeWindowType.GUID;
            Guid clsid = typeof(VsCodeWindowClass).GUID;
            var compModel = _package.AsVsServiceProvider().GetComponentModel();
            var adapterService = compModel.GetService<IVsEditorAdaptersFactoryService>();

            var window = adapterService.CreateVsCodeWindowAdapter((IOleServiceProvider)_serviceProvider.GetService(typeof(IOleServiceProvider)));
            ErrorHandler.ThrowOnFailure(window.SetBuffer(textLines));
            ErrorHandler.ThrowOnFailure(window.SetBaseEditorCaption(null));
            ErrorHandler.ThrowOnFailure(window.GetEditorCaption(READONLYSTATUS.ROSTATUS_Unknown, out editorCaption));

            IVsUserData userData = textLines as IVsUserData;
            if (userData != null)
            {
                if (_promptEncodingOnLoad)
                {
                    var guid = VSConstants.VsTextBufferUserDataGuid.VsBufferEncodingPromptOnLoad_guid;
                    userData.SetData(ref guid, (uint)1);
                }
            }

            var textMgr = _package.AsVsServiceProvider().GetTextManager();

            var bufferEventListener = new TextBufferEventListener(compModel, textLines, textMgr, window);
            if (!createdDocData)
            {
                // we have a pre-created buffer, go ahead and initialize now as the buffer already
                // exists and is initialized.
                bufferEventListener.OnLoadCompleted(0);
            }

            cmdUI = VSConstants.GUID_TextEditorFactory;

            return Marshal.GetIUnknownForObject(window);
        }

        #endregion

        /// <summary>
        /// Listens for the text buffer to finish loading and then sets up our projection
        /// buffer.
        /// </summary>
        internal sealed class TextBufferEventListener : IVsTextBufferDataEvents
        {
            private readonly IVsTextLines _textLines;
            private readonly uint _cookie;
            private readonly IConnectionPoint _cp;
            private readonly IComponentModel _compModel;
            private readonly IVsTextManager _textMgr;
            private readonly IVsCodeWindow _window;

            public TextBufferEventListener(IComponentModel compModel, IVsTextLines textLines, IVsTextManager textMgr, IVsCodeWindow window)
            {
                _textLines = textLines;
                _compModel = compModel;
                _textMgr = textMgr;
                _window = window;

                var cpc = textLines as IConnectionPointContainer;
                var bufferEventsGuid = typeof(IVsTextBufferDataEvents).GUID;
                cpc.FindConnectionPoint(ref bufferEventsGuid, out _cp);
                _cp.Advise(this, out _cookie);
            }

            #region IVsTextBufferDataEvents

            public void OnFileChanged(uint grfChange, uint dwFileAttrs)
            {
            }

            public int OnLoadCompleted(int fReload)
            {
                _cp.Unadvise(_cookie);

                var adapterService = _compModel.GetService<IVsEditorAdaptersFactoryService>();
                ITextBuffer diskBuffer = adapterService.GetDocumentBuffer(_textLines);

                var factService = _compModel.GetService<IProjectionBufferFactoryService>();

                var contentRegistry = _compModel.GetService<IContentTypeRegistryService>();


                IContentType contentType = SniffContentType(diskBuffer) ??
                                           contentRegistry.GetContentType("HTML");

                var projBuffer = new PhpProjectionBuffer(contentRegistry, factService, diskBuffer, _compModel.GetService<IBufferGraphFactoryService>(), contentType);
                diskBuffer.ChangedHighPriority += projBuffer.DiskBufferChanged;
                diskBuffer.Properties.AddProperty(typeof(PhpProjectionBuffer), projBuffer);

                Guid langSvcGuid = typeof(PhpLanguageInfo).GUID;
                _textLines.SetLanguageServiceID(ref langSvcGuid);

                adapterService.SetDataBuffer(_textLines, projBuffer.ProjectionBuffer);

                IVsTextView view;
                ErrorHandler.ThrowOnFailure(_window.GetPrimaryView(out view));

                if (contentType != null && contentType.IsOfType("HTML"))
                {
                    var editAdapter = _compModel.GetService<IVsEditorAdaptersFactoryService>();
                    var newView = editAdapter.GetWpfTextView(view);
                    var intellisenseController = HtmlIntellisenseControllerProvider.GetOrCreateController(_compModel, newView);
                    intellisenseController.AttachKeyboardFilter();
                }

                return VSConstants.S_OK;
            }

            private IContentType SniffContentType(ITextBuffer diskBuffer)
            {
                // try and sniff the content type from a double extension, and if we can't
                // do that then default to HTML.
                IContentType contentType = null;
                ITextDocument textDocument;
                if (diskBuffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out textDocument))
                {
                    if (Path.GetExtension(textDocument.FilePath).Equals(PhpConstants.Php5FileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        var path = Path.GetFileNameWithoutExtension(textDocument.FilePath);
                        if (path.IndexOf('.') != -1)
                        {
                            string subExt = Path.GetExtension(path).Substring(1);

                            var fileExtRegistry = _compModel.GetService<IFileExtensionRegistryService>();

                            contentType = fileExtRegistry.GetContentTypeForExtension(subExt);
                        }
                    }
                }
                return contentType;
            }

            #endregion
        }
    }
}
