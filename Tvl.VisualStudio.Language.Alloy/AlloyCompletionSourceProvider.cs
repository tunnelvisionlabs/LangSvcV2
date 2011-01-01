namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

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

        ICompletionSource ICompletionSourceProvider.TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new AlloyCompletionSource(textBuffer, this);
        }
    }
}
