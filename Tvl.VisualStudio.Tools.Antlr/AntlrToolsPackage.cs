namespace Tvl.VisualStudio.Tools
{
    using System;
    using System.ComponentModel.Design;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideToolWindow(typeof(AstExplorer.AstExplorerToolWindowPane))]
    [ProvideMenuResource(1000, 1)]
    [Guid("81989F3D-7E1B-4A12-B307-1E8D000573AE")]
    internal class AntlrToolsPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            OleMenuCommandService commandService = (OleMenuCommandService)base.GetService(typeof(IMenuCommandService));
            if (commandService != null)
            {
                CommandID toolWindowCommandId = new CommandID(Constants.GuidAntlrToolsCmdSet, Constants.ToolWindowCommandId);
                MenuCommand command = new MenuCommand(ShowAstExplorerWindow, toolWindowCommandId);
                commandService.AddCommand(command);
            }
        }

        private void ShowAstExplorerWindow(object sender, EventArgs e)
        {
            try
            {
                ToolWindowPane pane = FindToolWindow(typeof(AstExplorer.AstExplorerToolWindowPane), 0, true);
                if (pane != null)
                    ErrorHandler.ThrowOnFailure(((IVsWindowFrame)pane.Frame).Show());
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;
            }
        }
    }
}
