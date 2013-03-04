namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(IIntellisenseControllerProvider))]
    [ContentType("HTML")]
    [Order]
    internal class HtmlIntellisenseControllerProvider : IIntellisenseControllerProvider
    {
        internal readonly ICompletionBroker _CompletionBroker;
        internal readonly IVsEditorAdaptersFactoryService _adaptersFactory;

        [ImportingConstructor]
        public HtmlIntellisenseControllerProvider(ICompletionBroker broker, IVsEditorAdaptersFactoryService editorAdapter)
        {
            _CompletionBroker = broker;
            _adaptersFactory = editorAdapter;
        }

        #region IIntellisenseControllerProvider Members

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            HtmlIntellisenseController controller = null;
            if (!textView.Properties.TryGetProperty<HtmlIntellisenseController>(typeof(HtmlIntellisenseController), out controller))
            {
                foreach (var buffer in subjectBuffers)
                {
                    if (buffer.Properties.ContainsProperty(typeof(PhpProjectionBuffer)))
                    { // it's one of our buffers
                        controller = new HtmlIntellisenseController(this, textView);
                        textView.Properties.AddProperty(typeof(HtmlIntellisenseController), controller);
                    }
                }
            }

            return controller;
        }

        #endregion

        internal static HtmlIntellisenseController GetOrCreateController(IComponentModel model, ITextView textView)
        {
            HtmlIntellisenseController controller;
            if (!textView.Properties.TryGetProperty<HtmlIntellisenseController>(typeof(HtmlIntellisenseController), out controller))
            {
                var intellisenseControllerProvider = (
                   from export in model.DefaultExportProvider.GetExports<IIntellisenseControllerProvider, IContentTypeMetadata>()
                   from exportedContentType in export.Metadata.ContentTypes
                   where exportedContentType.Equals("HTML", StringComparison.OrdinalIgnoreCase) && export.Value.GetType() == typeof(HtmlIntellisenseControllerProvider)
                   select export.Value
                ).First();
                controller = new HtmlIntellisenseController((HtmlIntellisenseControllerProvider)intellisenseControllerProvider, textView);
                textView.Properties.AddProperty(typeof(HtmlIntellisenseController), controller);
            }
            return controller;
        }

    }
}
