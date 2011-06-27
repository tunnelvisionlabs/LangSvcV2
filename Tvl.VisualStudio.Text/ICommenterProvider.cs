namespace Tvl.VisualStudio.Text
{
    using Microsoft.VisualStudio.Text.Editor;
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(Contracts.ICommenterProviderContracts))]
    public interface ICommenterProvider
    {
        ICommenter GetCommenter(ITextView textView);
    }
}
