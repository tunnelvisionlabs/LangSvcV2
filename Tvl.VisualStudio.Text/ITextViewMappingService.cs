namespace Tvl.VisualStudio.Text
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    public interface ITextViewMappingService
    {
        IEnumerable<IWpfTextView> GetViewsForBuffer(ITextBuffer buffer);
    }
}
