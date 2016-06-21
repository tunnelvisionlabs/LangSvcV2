namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideToolWindow(typeof(MarkdownPreviewToolWindowPane))]
    [ProvideMenuResource(1000, 1)]
    [Guid("3FB748F7-1844-4465-B6A4-5473CD36EDEF")]
    internal class MarkdownPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            Guid menuGroup = MarkdownConstants.GuidMarkdownPackageCmdSet;
            int commandId = MarkdownConstants.ShowMarkdownPreviewToolWindowCommandId;
            WpfToolWindowPane.ProvideToolWindowCommand<MarkdownPreviewToolWindowPane>(this, menuGroup, commandId);
        }
    }
}
