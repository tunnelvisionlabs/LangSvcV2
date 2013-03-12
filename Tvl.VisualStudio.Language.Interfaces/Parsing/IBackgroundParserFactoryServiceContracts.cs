namespace Tvl.VisualStudio.Language.Parsing
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text;

    [ContractClassFor(typeof(IBackgroundParserFactoryService))]
    public abstract class IBackgroundParserFactoryServiceContracts : IBackgroundParserFactoryService
    {
        IBackgroundParser IBackgroundParserFactoryService.GetBackgroundParser(ITextBuffer buffer)
        {
            Contract.Requires<ArgumentNullException>(buffer != null, "buffer");
            throw new NotImplementedException();
        }
    }
}
