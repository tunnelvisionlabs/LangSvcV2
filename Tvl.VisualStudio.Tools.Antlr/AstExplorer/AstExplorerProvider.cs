namespace JavaLanguageService.AntlrLanguage
{
    using System.ComponentModel.Composition;
    using Tvl.VisualStudio.Shell;

    [Export(typeof(IToolWindowProvider))]
    public sealed class AstExplorerProvider : IToolWindowProvider
    {
        public IToolWindow CreateWindow()
        {
            return new ToolWindow<AstExplorerControl>("AST Explorer", null);
        }
    }
}
