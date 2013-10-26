namespace Tvl.VisualStudio.MouseNavigation
{
    using COMException = System.Runtime.InteropServices.COMException;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using Guid = System.Guid;
    using IServiceProvider = System.IServiceProvider;
    using IVsUIShell = Microsoft.VisualStudio.Shell.Interop.IVsUIShell;
    using IWpfTextView = Microsoft.VisualStudio.Text.Editor.IWpfTextView;
    using MouseButton = System.Windows.Input.MouseButton;
    using MouseButtonEventArgs = System.Windows.Input.MouseButtonEventArgs;
    using MouseProcessorBase = Microsoft.VisualStudio.Text.Editor.MouseProcessorBase;
    using OLECMDEXECOPT = Microsoft.VisualStudio.OLE.Interop.OLECMDEXECOPT;
    using SVsUIShell = Microsoft.VisualStudio.Shell.Interop.SVsUIShell;
    using VSConstants = Microsoft.VisualStudio.VSConstants;

    internal class MouseNavigationProcessor : MouseProcessorBase
    {
        public MouseNavigationProcessor(IWpfTextView wpfTextView, IServiceProvider serviceProvider)
        {
            TextView = wpfTextView;
            ServiceProvider = serviceProvider;
        }

        private IWpfTextView TextView
        {
            get;
            set;
        }

        private IServiceProvider ServiceProvider
        {
            get;
            set;
        }

        public override void PostprocessMouseUp(MouseButtonEventArgs e)
        {
            uint cmdId;
            switch (e.ChangedButton)
            {
            case MouseButton.XButton1:
                cmdId = (uint)VSConstants.VSStd97CmdID.ShellNavBackward;
                break;

            case MouseButton.XButton2:
                cmdId = (uint)VSConstants.VSStd97CmdID.ShellNavForward;
                break;

            default:
                return;
            }

            try
            {
                IVsUIShell shell = (IVsUIShell)ServiceProvider.GetService(typeof(SVsUIShell));
                Guid cmdGroup = VSConstants.GUID_VSStandardCommandSet97;
                OLECMDEXECOPT cmdExecOpt = OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER;
                object obj = null;
                ErrorHandler.ThrowOnFailure(shell.PostExecCommand(cmdGroup, cmdId, (uint)cmdExecOpt, ref obj));
                e.Handled = true;
            }
            catch (COMException)
            {
            }
        }
    }
}
