namespace Tvl.VisualStudio.Shell.OutputWindow.Interfaces.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IOutputWindowPane))]
    public abstract class IOutputWindowPaneContracts : IOutputWindowPane
    {
        public string Name
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        public void Activate()
        {
        }

        public void Hide()
        {
        }

        public void Write(string text)
        {
            Contract.Requires<ArgumentNullException>(text != null);
        }

        public void WriteLine(string text)
        {
            Contract.Requires<ArgumentNullException>(text != null);
        }

        public void Dispose()
        {
        }
    }
}
