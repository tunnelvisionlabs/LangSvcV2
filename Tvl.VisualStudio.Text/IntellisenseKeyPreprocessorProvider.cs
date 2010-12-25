namespace Tvl.VisualStudio.Text
{
    using System.ComponentModel.Composition;
    using IKeyProcessorProvider = Microsoft.VisualStudio.Text.Editor.IKeyProcessorProvider;
    using IWpfTextView = Microsoft.VisualStudio.Text.Editor.IWpfTextView;
    using KeyProcessor = Microsoft.VisualStudio.Text.Editor.KeyProcessor;

    public class IntellisenseKeyPreprocessorProvider : IKeyProcessorProvider
    {
        [Import]
        private ICompletionTargetMapService CompletionTargetMapService
        {
            get;
            set;
        }

        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            var completionTarget = CompletionTargetMapService.GetCompletionTargetForTextView(wpfTextView);
            return wpfTextView.Properties.GetOrCreateSingletonProperty(() => new IntellisenseKeyPreprocessor(wpfTextView.TextBuffer, completionTarget));
        }
    }
}
