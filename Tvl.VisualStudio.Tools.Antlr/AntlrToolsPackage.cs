namespace Tvl.VisualStudio.Tools
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell;
    using Tvl.VisualStudio.Tools.AstExplorer;

    [PackageRegistration(UseManagedResourcesOnly = true)]

    /*
     * Options pages
     */
    //[ProvideLanguageService(languageService, strLanguageName, languageResourceID)]
    //[ProvideLanguageExtension(typeof(LanguageServiceType), ".extension")]

    /*
     * Tool windows
     */
    [ProvideToolWindow(typeof(AstExplorer.AstExplorerToolWindowPane))]

    // Menu commands
    [ProvideMenuResource(1000, 1)]
    [Guid("81989F3D-7E1B-4A12-B307-1E8D000573AE")]
    internal class AntlrToolsPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            WpfToolWindowPane.ProvideToolWindowCommand<AstExplorerToolWindowPane>(this, Constants.GuidAntlrToolsCmdSet, Constants.ToolWindowCommandId);
        }
    }
}
