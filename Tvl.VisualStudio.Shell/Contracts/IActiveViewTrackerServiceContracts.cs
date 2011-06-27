namespace Tvl.VisualStudio.Shell.Contracts
{
    using System;
    using System.Diagnostics.Contracts;
    using ITextView = Microsoft.VisualStudio.Text.Editor.ITextView;

    [ContractClassFor(typeof(IActiveViewTrackerService))]
    public abstract class IActiveViewTrackerServiceContracts : IActiveViewTrackerService
    {
        event EventHandler<ViewChangedEventArgs> IActiveViewTrackerService.ViewChanged
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

        ITextView IActiveViewTrackerService.ActiveView
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
