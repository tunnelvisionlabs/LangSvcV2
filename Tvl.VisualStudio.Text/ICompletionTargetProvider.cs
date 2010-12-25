namespace Tvl.VisualStudio.Text
{
    using ITextView = Microsoft.VisualStudio.Text.Editor.ITextView;

    public interface ICompletionTargetProvider
    {
        ICompletionTarget CreateCompletionTarget(ITextView textView);
    }
}
