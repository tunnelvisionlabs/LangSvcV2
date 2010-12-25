namespace Tvl.VisualStudio.Text
{
    using System.ComponentModel.Composition;
    using IMouseProcessor = Microsoft.VisualStudio.Text.Editor.IMouseProcessor;
    using IMouseProcessorProvider = Microsoft.VisualStudio.Text.Editor.IMouseProcessorProvider;
    using IWpfTextView = Microsoft.VisualStudio.Text.Editor.IWpfTextView;

    public class IntellisenseMouseProcessorProvider : IMouseProcessorProvider
    {
        [Import]
        private ICompletionTargetMapService CompletionTargetMapService
        {
            get;
            set;
        }

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            ICompletionTarget completionTarget = CompletionTargetMapService.GetCompletionTargetForTextView(wpfTextView);
            return wpfTextView.Properties.GetOrCreateSingletonProperty(() => new IntellisenseMouseProcessor(wpfTextView.TextBuffer, completionTarget));
        }
    }
}
