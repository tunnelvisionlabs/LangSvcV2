namespace Tvl.VisualStudio.Shell.OutputWindow
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(IOutputWindowPaneContracts))]
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
