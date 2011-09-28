namespace Tvl.VisualStudio.Shell.OutputWindow.Interfaces
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(Contracts.IOutputWindowPaneContracts))]
    public interface IOutputWindowPane : IDisposable
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
