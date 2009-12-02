namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using Tvl.VisualStudio.Shell;

    [Guid("DB018843-6710-47E8-A6AE-901F1E37E1A8")]
    internal class AstExplorerWindow : ToolWindow
    {
        private AstExplorerProvider _provider;

        public AstExplorerWindow(AstExplorerProvider provider)
            : base("AST Explorer", null)
        {
            this._provider = provider;
        }

        protected override FrameworkElement CreateVisualElement()
        {
            return new AstExplorerControl(this._provider);
        }
    }
}
