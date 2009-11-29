namespace JavaLanguageService.ShellServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;

    public interface IOpenedViewTrackerService
    {
        IEnumerable<ITextView> OpenedViews
        {
            get;
        }
    }
}
