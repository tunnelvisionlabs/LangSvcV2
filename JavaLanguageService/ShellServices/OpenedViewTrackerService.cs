namespace JavaLanguageService.ShellServices
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;

    [Export(typeof(IWpfTextViewCreationListener))]
    [Export(typeof(IOpenedViewTrackerService))]
    public class OpenedViewTrackerService : IWpfTextViewCreationListener, IOpenedViewTrackerService
    {
        private readonly List<ITextView> _openedViews = new List<ITextView>();

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
