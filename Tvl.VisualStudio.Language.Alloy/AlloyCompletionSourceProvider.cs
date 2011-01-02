namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell.Extensions;

    [Name("AlloyCompletionSourceProvider")]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Order(Before = "default")]
    [Export(typeof(ICompletionSourceProvider))]
    internal class AlloyCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get;
            private set;
        }

        [Import]
        public IGlyphService GlyphService
        {
            get;
            private set;
        }

        [Import]
        public SVsServiceProvider GlobalServiceProvider
        {
            get;
            private set;
        }

        public IVsExpansionManager ExpansionManager
        {
            get
            {
                return GlobalServiceProvider.GetExpansionManager();
            }
        }

        ICompletionSource ICompletionSourceProvider.TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new AlloyCompletionSource(textBuffer, this);
        }
    }
}
