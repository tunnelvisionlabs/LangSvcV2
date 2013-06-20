namespace Tvl.VisualStudio.Text.Contracts
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text.Editor;

    [ContractClassFor(typeof(ICommenterProvider))]
    public abstract class ICommenterProviderContracts : ICommenterProvider
    {
        public ICommenter GetCommenter(ITextView textView)
        {
            Contract.Requires<ArgumentNullException>(textView != null, "textView");

            throw new NotImplementedException();
        }
    }
}
