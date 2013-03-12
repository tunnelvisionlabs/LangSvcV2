namespace Tvl.VisualStudio.Text.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using System.Diagnostics.Contracts;

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
