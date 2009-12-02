namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System.ComponentModel.Composition;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell;

    [Export(typeof(IToolWindowProvider))]
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
