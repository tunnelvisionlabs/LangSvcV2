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
    using MouseButtonState = System.Windows.Input.MouseButtonState;
    using MouseProcessorBase = Microsoft.VisualStudio.Text.Editor.MouseProcessorBase;
    using OLECMDEXECOPT = Microsoft.VisualStudio.OLE.Interop.OLECMDEXECOPT;
    using SVsUIShell = Microsoft.VisualStudio.Shell.Interop.SVsUIShell;
    using UIElement = System.Windows.UIElement;
    using VSConstants = Microsoft.VisualStudio.VSConstants;

    internal class MouseNavigationProcessor : MouseProcessorBase
    {
        public MouseNavigationProcessor(IWpfTextView wpfTextView, IServiceProvider serviceProvider)
        {
            TextView = wpfTextView;
            ServiceProvider = serviceProvider;
            UIElement element = TextView as UIElement;
            if (element != null)
                element.MouseDown += UIElement_MouseDown;
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

        private void UIElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed)
                return;

            uint cmdId = (uint)VSConstants.VSStd97CmdID.ShellNavBackward;
            if (e.ChangedButton == MouseButton.XButton1)
            {
                cmdId = (uint)VSConstants.VSStd97CmdID.ShellNavBackward;
            }
            else if (e.ChangedButton == MouseButton.XButton2)
            {
                cmdId = (uint)VSConstants.VSStd97CmdID.ShellNavForward;
            }

            try
            {
                IVsUIShell shell = (IVsUIShell)ServiceProvider.GetService(typeof(SVsUIShell));
                Guid cmdGroup = VSConstants.GUID_VSStandardCommandSet97;
                OLECMDEXECOPT cmdExecOpt = OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER;
                object obj = null;
                ErrorHandler.ThrowOnFailure(shell.PostExecCommand(cmdGroup, cmdId, (uint)cmdExecOpt, ref obj));
            }
            catch (COMException)
            {
            }
        }
    }
}
