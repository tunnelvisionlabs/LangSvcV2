namespace Tvl.VisualStudio.Text
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    public interface ITextViewMappingService
    {
        [NotNull]
        IEnumerable<IWpfTextView> GetViewsForBuffer([NotNull] ITextBuffer buffer);
    }
}
