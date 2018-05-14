namespace Tvl.VisualStudio.Shell
{
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using IVsDropdownBar = Microsoft.VisualStudio.TextManager.Interop.IVsDropdownBar;
    using IVsDropdownBarClient = Microsoft.VisualStudio.TextManager.Interop.IVsDropdownBarClient;
    using IVsDropdownBarManager = Microsoft.VisualStudio.TextManager.Interop.IVsDropdownBarManager;

    public static class IVsDropdownBarManagerExtensions
    {
        public static IVsDropdownBar GetDropdownBar([NotNull] this IVsDropdownBarManager dropdownBarManager)
        {
            Requires.NotNull(dropdownBarManager, nameof(dropdownBarManager));

            IVsDropdownBar dropdownBar;
            if (ErrorHandler.Failed(dropdownBarManager.GetDropdownBar(out dropdownBar)))
                return null;

            return dropdownBar;
        }

        public static IVsDropdownBarClient GetDropdownBarClient([NotNull] this IVsDropdownBarManager dropdownBarManager)
        {
            Requires.NotNull(dropdownBarManager, nameof(dropdownBarManager));

            IVsDropdownBar dropdownBar = GetDropdownBar(dropdownBarManager);
            IVsDropdownBarClient client;
            if (dropdownBar == null || ErrorHandler.Failed(dropdownBar.GetClient(out client)))
                return null;

            return client;
        }
    }
}
