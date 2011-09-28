namespace Tvl.VisualStudio.Shell.OutputWindow.Interfaces
{
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(Contracts.IOutputWindowServiceContracts))]
    public interface IOutputWindowService
    {
        IOutputWindowPane TryGetPane(string name);
    }
}
