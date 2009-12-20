namespace Tvl.VisualStudio.Shell.OutputWindow
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IOutputWindowService))]
    public sealed class IOutputWindowServiceContracts : IOutputWindowService
    {
        IOutputWindowPane IOutputWindowService.TryGetPane(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length <= 0)
                throw new ArgumentException();
            Contract.EndContractBlock();

            throw new NotImplementedException();
        }
    }
}
