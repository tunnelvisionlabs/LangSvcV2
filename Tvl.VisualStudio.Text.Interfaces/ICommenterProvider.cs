namespace Tvl.VisualStudio.Text
{
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text.Editor;

    [ContractClass(typeof(Contracts.ICommenterProviderContracts))]
    public interface ICommenterProvider
    {
        ICommenter GetCommenter(ITextView textView);
    }
}
