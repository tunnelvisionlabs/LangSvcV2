namespace Tvl.VisualStudio.Text.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public interface IEditorNavigationSourceAggregatorFactoryService
    {
        IEditorNavigationSourceAggregator CreateEditorNavigationSourceAggregator(ITextBuffer textBuffer);
    }
}
