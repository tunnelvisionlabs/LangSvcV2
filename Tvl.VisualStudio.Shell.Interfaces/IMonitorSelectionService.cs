namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text.Editor;

    [ContractClass(typeof(Contracts.IMonitorSelectionServiceContracts))]
    public interface IMonitorSelectionService
    {
        event EventHandler<ViewChangedEventArgs> ViewChanged;

        ITextView CurrentView
        {
            get;
        }
    }
}
