namespace Tvl.VisualStudio.Shell
{
    using System;
    using Microsoft.VisualStudio.Text.Editor;

    public interface IMonitorSelectionService
    {
        event EventHandler<ViewChangedEventArgs> ViewChanged;

        ITextView CurrentView
        {
            get;
        }
    }
}
