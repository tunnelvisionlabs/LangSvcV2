namespace Tvl.VisualStudio.Shell.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Media.Imaging;
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
    [ContentType("antlr")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    public sealed class ToolWindowService : IVsTextViewCreationListener
    {
        public const string ToolWindowCommandPrefix = "Tvl.ToolWindowService.Show";
        public const int TabImageHeight = 0x10;
        public const int TabImageWidth = 0x10;

        private bool created;
        private readonly Dictionary<uint, ToolWindowRegistrationInfo> _registeredToolWindows = new Dictionary<uint, ToolWindowRegistrationInfo>();
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
            IOleServiceProvider serviceProvider = GlobalServiceProvider.GetService<IOleServiceProvider>();
            IVsProfferCommands3 profferCommands = serviceProvider.TryGetGlobalService<EnvDTE.IVsProfferCommands, IVsProfferCommands3>();
            EnvDTE.DTE dte = serviceProvider.TryGetGlobalService<EnvDTE._DTE, EnvDTE.DTE>();

            int index = 0;
            foreach (var lazyProvider in ToolWindowProviders)
            {
                index++;
                var provider = lazyProvider.Value;
                var registration = new ToolWindowRegistrationInfo(provider.GetType(), lazyProvider.Metadata);

                var toolWindow = provider.CreateWindow();
                IVsUIElementPane vsToolWindow = toolWindow as IVsUIElementPane;
                if (vsToolWindow == null)
                    vsToolWindow = new ToolWindowAdapter(toolWindow);

                __VSCREATETOOLWIN creationFlags = __VSCREATETOOLWIN.CTW_fInitNew | __VSCREATETOOLWIN.CTW_fForceCreate;
                //if (!toolWindow.Transient)
                //    creationFlags |= __VSCREATETOOLWIN.CTW_fForceCreate;

                //bool hasToolbar = false;
                //if (hasToolbar)
                //    creationFlags |= __VSCREATETOOLWIN.CTW_fToolbarHost;

                //if (toolWindow.MultiInstance)
                //    creationFlags |= __VSCREATETOOLWIN.CTW_fMultiInstance;

                uint instance = 0;
                IVsWindowFrame windowFrame = null;
                string caption = toolWindow.Caption;
                Guid empty = Guid.Empty;
                Guid windowGuid = registration.Guid;
                int[] defaultPosition = null;

                ErrorHandler.ThrowOnFailure(shell.CreateToolWindow((uint)creationFlags, instance, vsToolWindow, ref empty, ref windowGuid, ref empty, serviceProvider, caption, defaultPosition, out windowFrame));
                if (windowFrame != null)
                {
                    if (toolWindow.Icon != null)
                    {
                        var icon = toolWindow.Icon;

                        if (icon.PixelWidth == TabImageWidth && icon.PixelHeight == TabImageHeight)
                        {
                            int stride = icon.Format.BitsPerPixel * icon.PixelWidth;
                            byte[] pixels = new byte[stride * icon.PixelHeight];
                            icon.CopyPixels(pixels, stride, 0);
                            icon = BitmapSource.Create(16, 16, 96.0, 96.0, icon.Format, null, pixels, stride);
                            windowFrame.SetProperty((int)__VSFPROPID4.VSFPROPID_TabImage, icon);
                        }
                        else
                        {
                            Trace.WriteLine(string.Format("The icon for the {0} window could not be used because it was not {1}x{2}px.", toolWindow.Caption, TabImageWidth, TabImageHeight));
                        }
                    }

                    dynamic commands = dte.CommandBars;
                    var bar1 = commands["MenuBar"];

                    // Microsoft.VisualStudio.ComponentModelHost.HostPackage
                    Guid package = new Guid("{49d12072-378b-4ffa-a09e-40e0b5d097cc}");
                    Guid group = typeof(ToolWindowService).GUID;
                    string pszCmdNameCanonical = registration.CommandName;
                    uint pdwCmdId;
                    string pszCmdNameLocalized = registration.CommandName;
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
                    {
                        _registeredToolWindows.Add(pdwCmdId, new ToolWindowRegistrationInfo(provider.GetType(), lazyProvider.Metadata));

                        if (hr == VSConstants.S_OK)
                        {
                            var dteCommands = dte.Commands;
                            var dteCommand = dteCommands.Item(pszCmdNameCanonical);

                            CommandBarPopup viewMenu = bar1.Controls["View"];
                            CommandBarPopup otherWindows = (CommandBarPopup)viewMenu.Controls["Other Windows"];
                            CommandBarButton control = (CommandBarButton)dteCommand.AddControl(otherWindows.Controls.Parent);
                            control.Visible = true;
                            control.Enabled = true;
                            control.Caption = pszBtnText;
                        }
                    }
                }
            }

            // Remove commands for tool windows that are no longer present
            IVsCmdNameMapping commandNameMapping = serviceProvider.TryGetGlobalService<SVsCmdNameMapping, IVsCmdNameMapping>();
            if (commandNameMapping != null)
            {
                foreach (var command in commandNameMapping.GetCommandNames())
                {
                    if (command.StartsWith(ToolWindowCommandPrefix) && !_registeredToolWindows.Any(registered=>registered.Value.CommandName == command))
                    {
                        profferCommands.RemoveNamedCommand(command);
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
                    ToolWindowRegistrationInfo windowInfo;
                    if (!toolWindowService._registeredToolWindows.TryGetValue(commandId, out windowInfo))
                        return false;

                    IVsWindowFrame frame;
                    Guid slot = windowInfo.Guid;
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

        private sealed class ToolWindowRegistrationInfo
        {
            public ToolWindowRegistrationInfo(Type providerType, IToolWindowMetadata metadata)
            {
                if (providerType == null)
                    throw new ArgumentNullException("providerType");
                if (metadata == null)
                    throw new ArgumentNullException("metadata");
                Contract.EndContractBlock();

                this.Guid = providerType.GUID;
                this.Name = metadata.Name;
                this.CommandName = ToolWindowCommandPrefix + metadata.Name;
            }

            public Guid Guid
            {
                get;
                private set;
            }

            public string Name
            {
                get;
                private set;
            }

            public string CommandName
            {
                get;
                private set;
            }
        }
    }
}
