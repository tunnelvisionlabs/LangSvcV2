namespace Tvl.VisualStudio.Language.Intellisense
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell.Extensions;

    public abstract class CompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        public IDispatcherGlyphService GlyphService
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

        public abstract ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer);
    }
}
