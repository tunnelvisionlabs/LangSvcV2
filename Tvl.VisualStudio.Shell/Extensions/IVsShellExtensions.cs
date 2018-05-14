namespace Tvl.VisualStudio.Shell
{
    using JetBrains.Annotations;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using Guid = System.Guid;
    using IVsPackage = Microsoft.VisualStudio.Shell.Interop.IVsPackage;
    using IVsShell = Microsoft.VisualStudio.Shell.Interop.IVsShell;
    using Package = Microsoft.VisualStudio.Shell.Package;

    public static class IVsShellExtensions
    {
        public static T LoadPackage<T>([NotNull] this IVsShell shell)
            where T : Package
        {
            Requires.NotNull(shell, nameof(shell));

            Guid guid = typeof(T).GUID;
            IVsPackage package;
            ErrorHandler.ThrowOnFailure(shell.LoadPackage(ref guid, out package));
            return (T)package;
        }
    }
}
