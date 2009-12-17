namespace JavaLanguageService.AntlrLanguage
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(AntlrConstants.AntlrContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class IntellisenseCommandFilterProvider : IVsTextViewCreationListener
    {
        [Import]
        private IVsEditorAdaptersFactoryService EditorAdaptersFactoryService
        {
            get;
            set;
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            IntellisenseCommandFilter filter = new IntellisenseCommandFilter(textViewAdapter);
            filter.Enabled = true;
            textView.Closed += (sender, e) => filter.Dispose();
        }
    }
}
