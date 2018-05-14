namespace Tvl.VisualStudio.Text
{
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text.Editor;

    public interface ICommenterProvider
    {
        ICommenter GetCommenter([NotNull] ITextView textView);
    }
}
