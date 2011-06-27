namespace Tvl.VisualStudio.Text
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    [ContractClass(typeof(Contracts.ITextViewMappingServiceContracts))]
    public interface ITextViewMappingService
    {
        IEnumerable<IWpfTextView> GetViewsForBuffer(ITextBuffer buffer);
    }
}
