namespace Tvl.VisualStudio.Shell
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text.Editor;

    public interface IOpenedViewTrackerService
    {
        [NotNull]
        IEnumerable<ITextView> OpenedViews
        {
            get;
        }
    }
}
