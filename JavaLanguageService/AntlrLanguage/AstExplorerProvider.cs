namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JavaLanguageService.Panes;
    using System.ComponentModel.Composition;

    [Export(typeof(IToolWindowProvider))]
    public sealed class AstExplorerProvider : IToolWindowProvider
    {
        public IToolWindow CreateWindow()
        {
            return new ToolWindow<AstExplorerControl>("AST Explorer", null);
        }
    }
}
