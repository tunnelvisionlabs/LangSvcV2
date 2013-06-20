namespace Tvl.VisualStudio.InheritanceMargin.CommandTranslation
{
    using System;
    using Tvl.VisualStudio.Shell;

    using Application = System.Windows.Application;
    using COMException = System.Runtime.InteropServices.COMException;
    using IInputElement = System.Windows.IInputElement;
    using IOleCommandTarget = Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
    using IVsUIShell = Microsoft.VisualStudio.Shell.Interop.IVsUIShell;
    using Marshal = System.Runtime.InteropServices.Marshal;
    using Mouse = System.Windows.Input.Mouse;
    using OLECMD = Microsoft.VisualStudio.OLE.Interop.OLECMD;
    using OLECMDF = Microsoft.VisualStudio.OLE.Interop.OLECMDF;
    using OLECMDTEXT = Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT;
    using OLECMDTEXTF = Microsoft.VisualStudio.OLE.Interop.OLECMDTEXTF;
    using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
    using Point = System.Windows.Point;
    using POINTS = Microsoft.VisualStudio.Shell.Interop.POINTS;
    using RoutedCommand = System.Windows.Input.RoutedCommand;
    using SVsServiceProvider = Microsoft.VisualStudio.Shell.SVsServiceProvider;
    using VSConstants = Microsoft.VisualStudio.VSConstants;

    public static class CommandRouter
    {
        private static SVsServiceProvider ServiceProvider
        {
            get
            {
                return InheritanceMarginPackage.Instance.ServiceProvider;
            }
        }

        public static void DisplayContextMenu(Guid menuGroup, int contextMenuId, IInputElement routing)
        {
            Point position = Mouse.GetPosition(Application.Current.MainWindow);
            Point point2 = Application.Current.MainWindow.PointToScreen(position);
            ContextMenuRouter pCmdTrgtActive = new ContextMenuRouter(routing);
            IVsUIShell service = ServiceProvider.GetUIShell();
            POINTS[] pos = new POINTS[1];
            pos[0].x = (short)point2.X;
            pos[0].y = (short)point2.Y;
            Guid rclsidActive = menuGroup;
            service.ShowContextMenu(0, ref rclsidActive, contextMenuId, pos, pCmdTrgtActive);
        }

        private static int RouteExec(IInputElement routing, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return RouteExec(
                (args, command) => command.CanExecute(args, routing),
                (args, command) => command.Execute(args, routing),
                ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        private static int RouteExec(Func<CommandTargetParameters, RoutedCommand, bool> canExecuteFunc, Action<CommandTargetParameters, RoutedCommand> executeFunc, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            RoutedCommand command = InheritanceMarginPackage.Instance.FindCommand(pguidCmdGroup, nCmdID);
            if (command == null)
                return (int)OleConstants.MSOCMDERR_E_UNKNOWNGROUP;

            CommandTargetParameters @params = CommandTargetParameters.CreateInstance(nCmdID);
            @params.InArgs = (pvaIn == IntPtr.Zero) ? null : Marshal.GetObjectForNativeVariant(pvaIn);
            if (canExecuteFunc(@params, command))
            {
                try
                {
                    executeFunc(@params, command);
                    return VSConstants.S_OK;
                }
                catch (COMException exception)
                {
                    return exception.ErrorCode;
                }
            }

            return (int)OleConstants.MSOCMDERR_E_NOTSUPPORTED;
        }

        private static int RouteQueryStatus(IInputElement routing, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return RouteQueryStatus((args, command) => command.CanExecute(args, routing), ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private static int RouteQueryStatus(Func<CommandTargetParameters, RoutedCommand, bool> canExecuteFunc, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            RoutedCommand command = InheritanceMarginPackage.Instance.FindCommand(pguidCmdGroup, prgCmds[0].cmdID);
            if (command == null)
                return (int)OleConstants.MSOCMDERR_E_UNKNOWNGROUP;

            string commandText = GetCommandText(pCmdText);
            CommandTargetParameters @params = CommandTargetParameters.CreateInstance(prgCmds[0].cmdID, commandText);
            if (!canExecuteFunc(@params, command))
                return (int)OleConstants.MSOCMDERR_E_NOTSUPPORTED;

            prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED;
            prgCmds[0].cmdf |= @params.Enabled ? (uint)OLECMDF.OLECMDF_ENABLED : 0;
            prgCmds[0].cmdf |= !@params.Visible ? (uint)OLECMDF.OLECMDF_INVISIBLE : 0;
            prgCmds[0].cmdf |= @params.Pressed ? (uint)OLECMDF.OLECMDF_LATCHED : 0;
            if (@params.Text == null)
                @params.Text = string.Empty;

            if (commandText != @params.Text)
                SetCommandText(pCmdText, @params.Text);

            return VSConstants.S_OK;
        }

        private static string GetCommandText(IntPtr structPtr)
        {
            if (structPtr == IntPtr.Zero)
                return string.Empty;

            OLECMDTEXT olecmdtext = (OLECMDTEXT)Marshal.PtrToStructure(structPtr, typeof(OLECMDTEXT));
            if (olecmdtext.cwActual == 0)
                return string.Empty;

            IntPtr offset = Marshal.OffsetOf(typeof(OLECMDTEXT), "rgwz");
            IntPtr ptr = (IntPtr)((long)structPtr + (long)offset);
            return Marshal.PtrToStringUni(ptr, (int)olecmdtext.cwActual - 1);
        }

        public static void SetCommandText(IntPtr pCmdTextInt, string text)
        {
            if (text != null)
            {
                OLECMDTEXT olecmdtext = (OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(OLECMDTEXT));
                if ((olecmdtext.cmdtextf & (uint)OLECMDTEXTF.OLECMDTEXTF_NAME) == 0)
                    return;

                char[] source = text.ToCharArray();
                IntPtr bufferOffset = Marshal.OffsetOf(typeof(OLECMDTEXT), "rgwz");
                IntPtr lengthOffset = Marshal.OffsetOf(typeof(OLECMDTEXT), "cwActual");
                int length = Math.Min(((int)olecmdtext.cwBuf) - 1, source.Length);

                // copy the new text
                long bufferAddress = (long)pCmdTextInt + (long)bufferOffset;
                Marshal.Copy(source, 0, (IntPtr)bufferAddress, length);
                // null terminator
                Marshal.WriteInt16(pCmdTextInt, (int)bufferOffset + length * 2, 0);
                // length including null terminator
                Marshal.WriteInt32(pCmdTextInt, (int)lengthOffset, length + 1);
            }
        }

        private class ContextMenuRouter : IOleCommandTarget
        {
            private readonly IInputElement _route;

            public ContextMenuRouter(IInputElement route)
            {
                this._route = route;
            }

            public IInputElement Route
            {
                get
                {
                    return _route;
                }
            }

            #region IOleCommandTarget Members

            public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
            {
                return RouteExec(Route, ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }

            public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
            {
                return RouteQueryStatus(Route, ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }

            #endregion
        }
    }
}
