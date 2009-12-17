namespace Tvl.VisualStudio.Shell.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    using Microsoft.VisualStudio.Text.Editor;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell.Extensions;
    using vsCommandControlType = EnvDTE80.vsCommandControlType;
    using Microsoft.VisualStudio.Shell;
    using System.ComponentModel.Design;

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    public sealed class ToolWindowService : IVsTextViewCreationListener
    {
        public bool created;

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

        private void CreateWindows()
        {
            IVsUIShell shell = (IVsUIShell)GlobalServiceProvider.GetService<SVsUIShell>();
            int index = 0;
            IOleServiceProvider serviceProvider = (IOleServiceProvider)GlobalServiceProvider.GetService(typeof(IOleServiceProvider));
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
                        //Guid package = new Guid("{49d12072-378b-4ffa-a09e-40e0b5d097cc}");
                        // Microsoft.VisualStudio.ExtensionManager.VSPackage.ExtensionManagerPackage
                        Guid package = new Guid("{E7576C05-1874-450c-9E98-CF3A0897A069}");
                        //package = new Guid("{AD1A73B0-C489-4c9c-B1FE-EEA54CD19A4F}");
                        Guid group = typeof(ToolWindowService).GUID;
                        //group = new Guid("{ADC1BC7B-958B-4548-9F9F-10FC49099825}");
                        //group = VSConstants.VsStd2010;
                        //Guid group = new Guid("{23162FF2-3C3F-11d2-890A-0060083196C6}");
                        string pszCmdNameCanonical = "Tvl.ToolWindowService.Show" + lazyProvider.Metadata.Name;
                        uint pdwCmdId = 1;
                        string pszCmdNameLocalized = pszCmdNameCanonical;
                        string pszBtnText = string.Format("Show the {0} window", caption);
                        string pszCmdTooltip = string.Format("Show the {0} window", caption);
                        pszCmdTooltip = null;
                        string pszSatelliteDLL = null;
                        uint dwBitmapResourceId = 0;
                        uint dwBitmapImageIndex = 0;
                        CommandOptions dwCmdFlagsDefault = CommandOptions.DynamicVisibility;
                        Guid[] rgguidUIContexts = { };
                        CommandType dwUIElementType = CommandType.Button;

                        profferCommands.RemoveNamedCommand(pszCmdNameCanonical);

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

                        var dteCommands = dte.Commands;
                        var dteCommand = dteCommands.Item(pszCmdNameCanonical);

                        Guid commandGuid;
                        if (Guid.TryParse(dteCommand.Guid, out commandGuid) && commandGuid != group)
                        {
                            group = commandGuid;
                        }

                        if (dteCommand.ID != (int)pdwCmdId)
                        {
                            pdwCmdId = (uint)dteCommand.ID;
                        }

                        IVsShell vsshell = serviceProvider.TryGetGlobalService<SVsShell, IVsShell>();
                        IVsPackage hostpackage;
                        if (ErrorHandler.Succeeded(vsshell.IsPackageLoaded(ref package, out hostpackage)))
                        {
                            Package hostPackage = hostpackage as Package;
                            if (hostpackage != null)
                            {
                                var mcs = hostPackage.GetService<IMenuCommandService>();
                                if (mcs != null)
                                {
                                    Guid menuGroup = group;
                                    CommandID showWindowId = new CommandID(menuGroup, (int)pdwCmdId);
                                    EventHandler invokeHandler = (sender, e) =>
                                        {
                                            windowFrame.Show();
                                        };
                                    EventHandler changeHandler =
                                        (sender, e) =>
                                        {
                                            return;
                                        };
                                    EventHandler beforeQueryStatus =
                                        (sender, e) =>
                                        {
                                            return;
                                        };
                                    OleMenuCommand command = new OleMenuCommand(invokeHandler, changeHandler, beforeQueryStatus, showWindowId);
                                    mcs.AddCommand(command);
                                }
                            }
                        }

                        Guid mainMenu = VsMenus.guidSHLMainMenu;
                        uint IDM_VS_MENU_VIEW = 0x0082;
                        object commandBar;
                        hr = profferCommands.FindCommandBar(null, ref mainMenu, IDM_VS_MENU_VIEW, out commandBar);
                        dynamic viewMenu = bar1.Controls["View"];
                        dynamic control = dteCommand.AddControl(viewMenu.Controls.Parent);
                        control.Visible = true;
                        control.Enabled = true;
                        control.Caption = pszBtnText;
                    }

                    //windowFrame.ShowNoActivate();
                }
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
            catch
            {
            }
        }
    }
}
