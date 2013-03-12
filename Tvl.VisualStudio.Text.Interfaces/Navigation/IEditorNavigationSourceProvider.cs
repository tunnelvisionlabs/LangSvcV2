namespace Tvl.VisualStudio.Text.Navigation
{
    using Microsoft.VisualStudio.Text;

    public interface IEditorNavigationSourceProvider
    {
        IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer);
    }
}
