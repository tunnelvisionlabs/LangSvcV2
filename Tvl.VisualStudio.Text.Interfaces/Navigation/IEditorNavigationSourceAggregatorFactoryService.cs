namespace Tvl.VisualStudio.Text.Navigation
{
    using Microsoft.VisualStudio.Text;

    public interface IEditorNavigationSourceAggregatorFactoryService
    {
        IEditorNavigationSourceAggregator CreateEditorNavigationSourceAggregator(ITextBuffer textBuffer);
    }
}
