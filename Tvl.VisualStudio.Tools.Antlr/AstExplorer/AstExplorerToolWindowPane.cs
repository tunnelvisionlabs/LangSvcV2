namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Controls;
    using Tvl.VisualStudio.Shell;

    [Guid("5201563E-22F8-404F-A515-75EFA1A4406D")]
    internal class AstExplorerToolWindowPane : WpfToolWindowPane
    {
        public AstExplorerToolWindowPane()
        {
            this.Caption = "AST Explorer";
        }

        protected override Control CreateToolWindowControl()
        {
            AstExplorerControl control = new AstExplorerControl(GlobalServiceProvider);
            return control;
        }
    }
}
