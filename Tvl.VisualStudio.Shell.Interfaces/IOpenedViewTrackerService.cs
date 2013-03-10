namespace Tvl.VisualStudio.Shell
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text.Editor;

    [ContractClass(typeof(Contracts.IOpenedViewTrackerServiceContracts))]
    public interface IOpenedViewTrackerService
    {
        IEnumerable<ITextView> OpenedViews
        {
            get;
        }
    }
}
