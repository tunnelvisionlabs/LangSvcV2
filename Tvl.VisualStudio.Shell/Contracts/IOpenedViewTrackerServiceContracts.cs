namespace Tvl.VisualStudio.Shell.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using ITextView = Microsoft.VisualStudio.Text.Editor.ITextView;

    [ContractClassFor(typeof(IOpenedViewTrackerService))]
    public abstract class IOpenedViewTrackerServiceContracts : IOpenedViewTrackerService
    {
        public IEnumerable<ITextView> OpenedViews
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ITextView>>() != null);

                throw new NotImplementedException();
            }
        }
    }
}
