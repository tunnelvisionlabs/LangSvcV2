namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System.ComponentModel.Composition;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell;

    [Export(typeof(IToolWindowProvider))]
    [Name("ASTExplorer")]
    [Guid("DB018843-6710-47E8-A6AE-901F1E37E1A8")]
    internal sealed class AstExplorerProvider : IToolWindowProvider
    {
        [Import]
        internal IActiveViewTrackerService ActiveViewTrackerService
        {
            get;
            private set;
        }

        [Import]
        internal IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            private set;
        }

        [Import]
        internal IAstReferenceTaggerProvider AstReferenceTaggerProvider
        {
            get;
            private set;
        }

        public IToolWindow CreateWindow()
        {
            return new AstExplorerWindow(this);
        }
    }
}
