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
        private IEnumerable<Lazy<IToolWindowProvider>> ToolWindowProviders
        {
            get;
            set;
        }

        private void CreateWindows()
        {
            IVsUIShell shell = (IVsUIShell)GlobalServiceProvider.GetService<SVsUIShell>();

            IOleServiceProvider serviceProvider = (IOleServiceProvider)GlobalServiceProvider.GetService(typeof(IOleServiceProvider));
            foreach (var lazyProvider in ToolWindowProviders)
            {
                var provider = lazyProvider.Value;
                var toolWindow = provider.CreateWindow();
                __VSCREATETOOLWIN creationFlags = __VSCREATETOOLWIN.CTW_fInitNew;
                if (!toolWindow.Transient)
                    creationFlags |= __VSCREATETOOLWIN.CTW_fForceCreate;

                bool hasToolbar = false;
                if (hasToolbar)
                    creationFlags |= __VSCREATETOOLWIN.CTW_fToolbarHost;

                if (toolWindow.MultiInstance)
                    creationFlags |= __VSCREATETOOLWIN.CTW_fMultiInstance;

                uint instance = 0;
                IVsWindowFrame windowFrame = null;
                string caption = toolWindow.Caption;
                Guid empty = Guid.Empty;
                Guid providerGuid = provider.GetType().GUID;
                ErrorHandler.ThrowOnFailure(shell.CreateToolWindow((uint)creationFlags, instance, toolWindow, ref empty, ref providerGuid, ref empty, serviceProvider, caption, null, out windowFrame));
                if (windowFrame != null)
                    windowFrame.ShowNoActivate();
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
