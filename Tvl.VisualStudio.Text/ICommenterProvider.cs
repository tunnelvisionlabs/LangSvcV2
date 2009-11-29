namespace Tvl.VisualStudio.Text
{
    using Microsoft.VisualStudio.Text.Editor;

    public interface ICommenterProvider
    {
        ICommenter GetCommenter(ITextView textView);
    }
}
