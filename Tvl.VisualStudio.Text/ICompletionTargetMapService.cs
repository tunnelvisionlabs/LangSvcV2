namespace Tvl.VisualStudio.Text
{
    using ITextView = Microsoft.VisualStudio.Text.Editor.ITextView;

    public interface ICompletionTargetMapService
    {
        ICompletionTarget GetCompletionTargetForTextView(ITextView textView);
    }
}
