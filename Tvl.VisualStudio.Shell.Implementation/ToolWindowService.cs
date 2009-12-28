namespace Tvl.VisualStudio.Shell.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.CommandBars;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell.Extensions;
    using IOleCommandTarget = Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("java")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    public sealed class ToolWindowService : IVsTextViewCreationListener
    {
        private bool created;
        private readonly Dictionary<uint, Guid> _registeredToolWindows = new Dictionary<uint, Guid>();
        private CommandTarget _commandTarget;

        [Import]
        private IServiceProvider GlobalServiceProvider
        {
            get;
            set;
        }

        [ImportMany]
        private IEnumerable<Lazy<IToolWindowProvider, IToolWindowMetadata>> ToolWindowProviders
        {
            get;
            set;
        }

        private void RegisterCommandTarget()
        {
            _commandTarget = new CommandTarget(this);
            _commandTarget.Enabled = true;
        }

        private void CreateWindows()
        {
            IVsUIShell shell = (IVsUIShell)GlobalServiceProvider.GetService<SVsUIShell>();
            int index = 0;
            IOleServiceProvider serviceProvider = GlobalServiceProvider.GetService<IOleServiceProvider>();
            foreach (var lazyProvider in ToolWindowProviders)
            {
                index++;
                var provider = lazyProvider.Value;

                var toolWindow = provider.CreateWindow();
                IVsUIElementPane vsToolWindow = toolWindow as IVsUIElementPane;
                if (vsToolWindow == null)
                    vsToolWindow = new ToolWindowAdapter(toolWindow);

                __VSCREATETOOLWIN creationFlags = __VSCREATETOOLWIN.CTW_fInitNew;
                //if (!toolWindow.Transient)
                //    creationFlags |= __VSCREATETOOLWIN.CTW_fForceCreate;

                bool hasToolbar = false;
                if (hasToolbar)
                    creationFlags |= __VSCREATETOOLWIN.CTW_fToolbarHost;

                //if (toolWindow.MultiInstance)
                //    creationFlags |= __VSCREATETOOLWIN.CTW_fMultiInstance;

                uint instance = 0;
                IVsWindowFrame windowFrame = null;
                string caption = toolWindow.Caption;
                Guid empty = Guid.Empty;
                Guid providerGuid = provider.GetType().GUID;
                ErrorHandler.ThrowOnFailure(shell.CreateToolWindow((uint)creationFlags, instance, vsToolWindow, ref empty, ref providerGuid, ref empty, serviceProvider, caption, null, out windowFrame));
                if (windowFrame != null)
                {
                    EnvDTE.DTE dte = serviceProvider.TryGetGlobalService<EnvDTE._DTE, EnvDTE.DTE>();
                    dynamic commands = dte.CommandBars;
                    var bar1 = commands["MenuBar"];

                    IVsProfferCommands3 profferCommands = serviceProvider.TryGetGlobalService<EnvDTE.IVsProfferCommands, IVsProfferCommands3>();
                    if (profferCommands != null)
                    {
                        // Microsoft.VisualStudio.ComponentModelHost.HostPackage
                        Guid package = new Guid("{49d12072-378b-4ffa-a09e-40e0b5d097cc}");
                        Guid group = typeof(ToolWindowService).GUID;
                        string pszCmdNameCanonical = "Tvl.ToolWindowService.Show" + lazyProvider.Metadata.Name;
                        uint pdwCmdId;
                        string pszCmdNameLocalized = pszCmdNameCanonical;
                        string pszBtnText = string.Format("{0}", caption);
                        string pszCmdTooltip = string.Format("Show the {0} window", caption);
                        string pszSatelliteDLL = null;
                        uint dwBitmapResourceId = 0;
                        uint dwBitmapImageIndex = 0;
                        CommandOptions dwCmdFlagsDefault = CommandOptions.None;
                        Guid[] rgguidUIContexts = { };
                        CommandType dwUIElementType = CommandType.Button;

                        int hr = profferCommands.AddNamedCommand2(
                            ref package,
                            ref group,
                            pszCmdNameCanonical,
                            out pdwCmdId,
                            pszCmdNameLocalized,
                            pszBtnText,
                            pszCmdTooltip,
                            pszSatelliteDLL,
                            dwBitmapResourceId,
                            dwBitmapImageIndex,
                            (uint)dwCmdFlagsDefault,
                            (uint)rgguidUIContexts.Length,
                            rgguidUIContexts,
                            (uint)dwUIElementType);

                        if (ErrorHandler.Succeeded(hr))
                            _registeredToolWindows.Add(pdwCmdId, providerGuid);

                        var dteCommands = dte.Commands;
                        var dteCommand = dteCommands.Item(pszCmdNameCanonical);

                        //Guid mainMenu = VsMenus.guidSHLMainMenu;
                        //uint IDM_VS_MENU_VIEW = 0x0082;
                        //object commandBar;
                        //hr = profferCommands.FindCommandBar(null, ref mainMenu, IDM_VS_MENU_VIEW, out commandBar);

                        CommandBarPopup viewMenu = bar1.Controls["View"];
                        CommandBarPopup otherWindows = (CommandBarPopup)viewMenu.Controls["Other Windows"];
                        CommandBarButton control = (CommandBarButton)dteCommand.AddControl(otherWindows.Controls.Parent);
                        control.Visible = true;
                        control.Enabled = true;
                        control.Caption = pszBtnText;
                    }
                }
            }

            if (ToolWindowProviders.Any())
            {
                RegisterCommandTarget();
            }
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            try
            {
                if (!created)
                {
                    created = true;
                    CreateWindows();
                }
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;
            }
        }

        private class CommandTarget : CommandFilter
        {
            private ToolWindowService toolWindowService;
            private uint _cookie;

            public CommandTarget(ToolWindowService toolWindowService)
            {
                this.toolWindowService = toolWindowService;
            }

            protected override bool HandlePreExec(ref Guid commandGroup, uint commandId, uint executionOptions, IntPtr pvaIn, IntPtr pvaOut)
            {
                if (commandGroup == typeof(ToolWindowService).GUID)
                {
                    IVsUIShell shell = (IVsUIShell)toolWindowService.GlobalServiceProvider.GetService<SVsUIShell>();

                    __VSCREATETOOLWIN flags = __VSCREATETOOLWIN.CTW_fForceCreate;
                    Guid slot;
                    if (!toolWindowService._registeredToolWindows.TryGetValue(commandId, out slot))
                        return false;

                    IVsWindowFrame frame;
                    if (ErrorHandler.Failed(shell.FindToolWindow((uint)flags, ref slot, out frame)))
                        return false;

                    frame.Show();
                    return true;
                }

                return false;
            }

            protected override CommandStatus QueryCommandStatus(ref Guid group, uint id)
            {
                if (group == typeof(ToolWindowService).GUID)
                {
                    if (toolWindowService._registeredToolWindows.ContainsKey(id))
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    return CommandStatus.Supported;
                }

                return base.QueryCommandStatus(ref group, id);
            }

            protected override IOleCommandTarget Connect()
            {
                IVsRegisterPriorityCommandTarget registerPct = (IVsRegisterPriorityCommandTarget)toolWindowService.GlobalServiceProvider.GetService<SVsRegisterPriorityCommandTarget>();
                registerPct.RegisterPriorityCommandTarget(0, this, out _cookie);
                return null;
            }

            protected override void Disconnect()
            {
                try
                {
                    if (_cookie != 0)
                    {
                        IVsRegisterPriorityCommandTarget registerPct = (IVsRegisterPriorityCommandTarget)toolWindowService.GlobalServiceProvider.GetService<SVsRegisterPriorityCommandTarget>();
                        registerPct.UnregisterPriorityCommandTarget(_cookie);
                    }
                }
                finally
                {
                    _cookie = 0;
                }
            }
        }
    }
}
