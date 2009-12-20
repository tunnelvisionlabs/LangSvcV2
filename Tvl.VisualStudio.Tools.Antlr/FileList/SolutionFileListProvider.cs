namespace Tvl.VisualStudio.Tools.FileList
{
    using System;
    using System.ComponentModel.Composition;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell;

    [Export(typeof(IToolWindowProvider))]
    [Name("SolutionFileList")]
    [Guid("3212FBB5-C9B1-463E-817C-23D969CDC1D6")]
    internal sealed class SolutionFileListProvider : IToolWindowProvider
    {
        [Import]
        internal IServiceProvider ServiceProvider
        {
            get;
            set;
        }

        public IToolWindow CreateWindow()
        {
            return new SolutionFileListWindow(this);
        }
    }
}
