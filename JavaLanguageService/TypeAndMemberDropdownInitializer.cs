namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using JavaLanguageService.Extensions;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio;

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(Constants.JavaContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    public sealed class TypeAndMemberDropdownInitializer : IVsTextViewCreationListener
    {
        [ImportMany]
        internal List<Lazy<IDropdownBarsProvider, IContentTypeMetadata>> _dropdownBarsProviders;

        [Import]
        internal IVsEditorAdaptersFactoryService _adaptorsService;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = _adaptorsService.GetWpfTextView(textViewAdapter);
            var bars = GatherDropdownBars(textView);
            if (bars.Count == 0)
                return;

            var dropdownManager = textViewAdapter.GetCodeWindow() as IVsDropdownBarManager;
            if (dropdownManager == null)
                return;

            //dropdownManager.AddDropdownBar(2, TypeAndMemberDropdownBars

            //IVsDropdownBar dropdownBar;
            //if (ErrorHandler.Failed(dropdownManager.GetDropdownBar(out dropdownBar)))
            //    return;


        }

        private IList<IDropdownBars> GatherDropdownBars(ITextView textView)
        {
            IList<IDropdownBars> bars = new List<IDropdownBars>();
            foreach (var lazy in _dropdownBarsProviders)
            {
                foreach (string str in lazy.Metadata.ContentTypes)
                {
                    if (textView.TextBuffer.ContentType.IsOfType(str))
                    {
                        IDropdownBars item = lazy.Value.GetDropdownBars(textView);
                        if (item != null)
                            bars.Add(item);

                        break;
                    }
                }
            }

            return bars;
        }
    }
}
