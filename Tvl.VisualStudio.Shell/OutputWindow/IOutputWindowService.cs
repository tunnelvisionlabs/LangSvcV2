namespace Tvl.VisualStudio.Shell.OutputWindow
{
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(IOutputWindowServiceContracts))]
    public interface IOutputWindowService
    {
        IOutputWindowPane TryGetPane(string name);
    }
}
