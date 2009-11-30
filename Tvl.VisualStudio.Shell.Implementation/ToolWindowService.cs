namespace Tvl.VisualStudio.Shell.Implementation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    public sealed class ToolWindowService
    {
        public IServiceProvider GlobalServiceProvider;
        public IVsUIShell VsUIShell;

        public void CreateWindows()
        {
            IOleServiceProvider serviceProvider = (IOleServiceProvider)GlobalServiceProvider.GetService(typeof(IOleServiceProvider));
            IEnumerable<IToolWindow> windows = null;
            foreach (var window in windows)
            {
                __VSCREATETOOLWIN creationFlags = 0;
                uint instance = 0;
                IVsWindowFrame windowFrame = null;
                string caption = "TestFrame";
                Guid zero = Guid.Empty;
                ErrorHandler.ThrowOnFailure(VsUIShell.CreateToolWindow((uint)creationFlags, instance, null, ref zero, ref zero, ref zero, serviceProvider, caption, null, out windowFrame));
            }
        }
    }
}
