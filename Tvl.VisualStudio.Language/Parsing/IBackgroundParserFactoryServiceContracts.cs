namespace Tvl.VisualStudio.Language.Parsing
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text;

    [ContractClassFor(typeof(IBackgroundParserFactoryService))]
    public sealed class IBackgroundParserFactoryServiceContracts : IBackgroundParserFactoryService
    {
        IBackgroundParser IBackgroundParserFactoryService.GetBackgroundParser(ITextBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            Contract.EndContractBlock();
            throw new NotImplementedException();
        }
    }
}
