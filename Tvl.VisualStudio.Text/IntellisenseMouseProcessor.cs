namespace Tvl.VisualStudio.Text
{
    using System.Windows.Input;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    public class IntellisenseMouseProcessor : MouseProcessorBase
    {
        private readonly ITextBuffer _textBuffer;
        private readonly ICompletionTarget _completionTarget;

        public IntellisenseMouseProcessor(ITextBuffer textBuffer, ICompletionTarget completionTarget)
        {
            _textBuffer = textBuffer;
            _completionTarget = completionTarget;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        public virtual ICompletionTarget CompletionTarget
        {
            get
            {
                return _completionTarget;
            }
        }

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var completionTarget = CompletionTarget;
            if (completionTarget != null)
            {
                completionTarget.DismissCompletion();
                completionTarget.DismissQuickInfo();
            }
        }

        public override void PreprocessMouseRightButtonDown(MouseButtonEventArgs e)
        {
            var completionTarget = CompletionTarget;
            if (completionTarget != null)
            {
                completionTarget.DismissCompletion();
                completionTarget.DismissQuickInfo();
            }
        }
    }
}
