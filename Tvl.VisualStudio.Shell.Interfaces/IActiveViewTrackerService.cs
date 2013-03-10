namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text.Editor;

    [ContractClass(typeof(Contracts.IActiveViewTrackerServiceContracts))]
    public interface IActiveViewTrackerService
    {
        event EventHandler<ViewChangedEventArgs> ViewChanged;

        event EventHandler<ViewChangedEventArgs> ViewWithMouseChanged;

        ITextView ActiveView
        {
            get;
        }

        ITextView ViewWithMouse
        {
            get;
        }
    }
}
