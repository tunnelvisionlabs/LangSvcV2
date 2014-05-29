namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using Microsoft.VisualStudio.Shell.Interop;

    // Stolen from Microsoft.RestrictedUsage.CSharp.Utilities in Microsoft.VisualStudio.CSharp.Services.Language.dll
    public static class RoslynUtilities
    {
        private static bool? roslynInstalled;

        public static bool IsRoslynInstalled(IServiceProvider serviceProvider)
        {
            if (roslynInstalled.HasValue)
                return roslynInstalled.Value;

            roslynInstalled = false;
            if (serviceProvider == null)
                return false;

            IVsShell vsShell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
            if (vsShell == null)
                return false;

            Guid guid = new Guid("6cf2e545-6109-4730-8883-cf43d7aec3e1");
            int isInstalled;
            if (vsShell.IsPackageInstalled(ref guid, out isInstalled) == 0 && isInstalled != 0)
                roslynInstalled = true;

            return roslynInstalled.Value;
        }
    }
}
