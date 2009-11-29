namespace JavaLanguageService.ShellServices
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;

    [Export(typeof(IWpfTextViewCreationListener))]
    [Export(typeof(IOpenedViewTrackerService))]
    public class OpenedViewTrackerService : IWpfTextViewCreationListener, IOpenedViewTrackerService
    {
        private List<ITextView> _openedViews;

        public IEnumerable<ITextView> OpenedViews
        {
            get
            {
                return _openedViews;
            }
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            _openedViews.Add(textView);
            textView.Closed += (sender, e) => _openedViews.Remove(textView);
        }
    }
}
