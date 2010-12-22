namespace Tvl.VisualStudio.Shell.Implementation
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using EventArgs = System.EventArgs;
    using ITextView = Microsoft.VisualStudio.Text.Editor.ITextView;
    using IVsEditorAdaptersFactoryService = Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService;
    using IVsTextView = Microsoft.VisualStudio.TextManager.Interop.IVsTextView;
    using IVsTextViewCreationListener = Microsoft.VisualStudio.Editor.IVsTextViewCreationListener;

    [Export(typeof(IVsTextViewCreationListener))]
    [Export(typeof(IOpenedViewTrackerService))]
    public class OpenedViewTrackerService : IVsTextViewCreationListener, IOpenedViewTrackerService
    {
        private readonly List<ITextView> _openedViews = new List<ITextView>();

        public IEnumerable<ITextView> OpenedViews
        {
            get
            {
                return _openedViews.ToArray();
            }
        }

        [Import]
        private IVsEditorAdaptersFactoryService EditorAdaptersFactoryService
        {
            get;
            set;
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            if (textView != null)
            {
                _openedViews.Add(textView);
                textView.Closed += HandleTextViewClosed;
            }
        }

        private void HandleTextViewClosed(object sender, EventArgs e)
        {
            ITextView view = sender as ITextView;
            if (view != null)
            {
                _openedViews.Remove(view);
                view.Closed -= HandleTextViewClosed;
            }
        }
    }
}
