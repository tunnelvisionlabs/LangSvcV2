namespace Tvl.VisualStudio.Text.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class AutoCommentService : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService;

        [ImportMany]
        public List<Lazy<ICommenterProvider, IContentTypeMetadata>> CommenterProviders
        {
            get;
            set;
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            var provider = CommenterProviders.FirstOrDefault(providerInfo => providerInfo.Metadata.ContentTypes.Any(contentType => textView.TextBuffer.ContentType.IsOfType(contentType)));
            if (provider != null)
            {
                var commenter = provider.Value.GetCommenter(textView);
                CommenterFilter filter = new CommenterFilter(textViewAdapter, textView, commenter);
                filter.Enabled = true;
                textView.Closed += (sender, e) => filter.Dispose();
            }
        }
    }
}
