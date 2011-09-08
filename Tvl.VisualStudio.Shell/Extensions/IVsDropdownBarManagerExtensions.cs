namespace Tvl.VisualStudio.Shell.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using IVsDropdownBarClient = Microsoft.VisualStudio.TextManager.Interop.IVsDropdownBarClient;
    using IVsDropdownBarManager = Microsoft.VisualStudio.TextManager.Interop.IVsDropdownBarManager;
    using IVsDropdownBar = Microsoft.VisualStudio.TextManager.Interop.IVsDropdownBar;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;

    public static class IVsDropdownBarManagerExtensions
    {
        public static IVsDropdownBar GetDropdownBar(this IVsDropdownBarManager dropdownBarManager)
        {
            Contract.Requires<ArgumentNullException>(dropdownBarManager != null, "dropdownBarManager");

            IVsDropdownBar dropdownBar;
            if (ErrorHandler.Failed(dropdownBarManager.GetDropdownBar(out dropdownBar)))
                return null;

            return dropdownBar;
        }

        public static IVsDropdownBarClient GetDropdownBarClient(this IVsDropdownBarManager dropdownBarManager)
        {
            Contract.Requires<ArgumentNullException>(dropdownBarManager != null, "dropdownBarManager");

            IVsDropdownBar dropdownBar = GetDropdownBar(dropdownBarManager);
            IVsDropdownBarClient client;
            if (dropdownBar == null || ErrorHandler.Failed(dropdownBar.GetClient(out client)))
                return null;

            return client;
        }
    }
}
