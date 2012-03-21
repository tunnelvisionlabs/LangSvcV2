namespace Tvl.VisualStudio.Shell.OutputWindow.Interfaces
{
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(Contracts.IOutputWindowPaneContracts))]
    public interface IOutputWindowPane
    {
        string Name
        {
            get;
            set;
        }

        void Activate();
        void Hide();
        void Write(string text);
        void WriteLine(string text);
    }
}
