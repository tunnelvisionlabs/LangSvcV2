namespace Tvl.VisualStudio.Language.Parsing
{
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text;

    [ContractClass(typeof(IBackgroundParserFactoryServiceContracts))]
    public interface IBackgroundParserFactoryService
    {
        IBackgroundParser GetBackgroundParser(ITextBuffer buffer);
    }
}
