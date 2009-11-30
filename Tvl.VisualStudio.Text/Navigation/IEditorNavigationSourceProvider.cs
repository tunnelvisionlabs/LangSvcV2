namespace Tvl.VisualStudio.Text
{
    using Microsoft.VisualStudio.Text;

    public interface IEditorNavigationSourceProvider
    {
        IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer);
    }
}
