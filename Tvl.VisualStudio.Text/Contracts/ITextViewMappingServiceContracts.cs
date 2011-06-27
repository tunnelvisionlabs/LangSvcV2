namespace Tvl.VisualStudio.Text.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    [ContractClassFor(typeof(ITextViewMappingService))]
    public abstract class ITextViewMappingServiceContracts : ITextViewMappingService
    {
        public IEnumerable<IWpfTextView> GetViewsForBuffer(ITextBuffer buffer)
        {
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");
            Contract.Ensures(Contract.Result<IEnumerable<IWpfTextView>>() != null);

            throw new NotImplementedException();
        }
    }
}
