namespace Tvl.VisualStudio.Shell.Contracts
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text.Editor;

    [ContractClassFor(typeof(IMonitorSelectionService))]
    public abstract class IMonitorSelectionServiceContracts : IMonitorSelectionService
    {
        event EventHandler<ViewChangedEventArgs> IMonitorSelectionService.ViewChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        ITextView IMonitorSelectionService.CurrentView
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
