namespace Tvl.VisualStudio.Shell.OutputWindow
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IOutputWindowService))]
    public sealed class IOutputWindowServiceContracts : IOutputWindowService
    {
        IOutputWindowPane IOutputWindowService.TryGetPane(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentException>(name.Length > 0);

            throw new NotImplementedException();
        }
    }
}
