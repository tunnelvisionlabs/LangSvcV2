/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * This source code has been modified from its original form.
 *
 * ***************************************************************************/

namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Projection;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using IComponentModel = Microsoft.VisualStudio.ComponentModelHost.IComponentModel;
    using IConnectionPoint = Microsoft.VisualStudio.OLE.Interop.IConnectionPoint;
    using IConnectionPointContainer = Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;
    using IVsCodeWindow = Microsoft.VisualStudio.TextManager.Interop.IVsCodeWindow;
    using IVsEditorAdaptersFactoryService = Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService;
    using IVsTextBufferDataEvents = Microsoft.VisualStudio.TextManager.Interop.IVsTextBufferDataEvents;
    using IVsTextLines = Microsoft.VisualStudio.TextManager.Interop.IVsTextLines;
    using IVsTextManager = Microsoft.VisualStudio.TextManager.Interop.IVsTextManager;
    using IVsTextView = Microsoft.VisualStudio.TextManager.Interop.IVsTextView;
    using Path = System.IO.Path;
    using VSConstants = Microsoft.VisualStudio.VSConstants;

    public abstract class PhpEditorFactory : EditorFactory
    {
        protected PhpEditorFactory(PhpLanguagePackage package, bool promptEncodingOnLoad)
            : base(package, promptEncodingOnLoad)
        {
        }

        protected override IVsCodeWindow CreateCodeView(string documentMoniker, IVsTextLines textLines, bool createdDocData, ref string editorCaption, ref Guid cmdUI)
        {
            IVsCodeWindow window = base.CreateCodeView(documentMoniker, textLines, createdDocData, ref editorCaption, ref cmdUI);

            var compModel = Package.AsVsServiceProvider().GetComponentModel();
            var textMgr = Package.AsVsServiceProvider().GetTextManager();
            var bufferEventListener = new TextBufferEventListener(compModel, textLines, textMgr, window);
            if (!createdDocData)
            {
                // we have a pre-created buffer, go ahead and initialize now as the buffer already
                // exists and is initialized.
                bufferEventListener.OnLoadCompleted(0);
            }

            return window;
        }

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

                var bufferTagAggregatorFactory = _compModel.GetService<IBufferTagAggregatorFactoryService>();

                IContentType contentType = SniffContentType(diskBuffer) ??
                                           contentRegistry.GetContentType("HTML");

                var projBuffer = new PhpProjectionBuffer(contentRegistry, factService, bufferTagAggregatorFactory, diskBuffer, _compModel.GetService<IBufferGraphFactoryService>(), contentType);
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
                    if (Path.GetExtension(textDocument.FilePath).Equals(PhpConstants.PhpFileExtension, StringComparison.OrdinalIgnoreCase)
                        || Path.GetExtension(textDocument.FilePath).Equals(PhpConstants.Php5FileExtension, StringComparison.OrdinalIgnoreCase))
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
