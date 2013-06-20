namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Intellisense;

    [Name("AlloyCompletionSourceProvider")]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Order(Before = "default")]
    [Export(typeof(ICompletionSourceProvider))]
    internal class AlloyCompletionSourceProvider : CompletionSourceProvider
    {
        [Import]
        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get;
            private set;
        }

        [Import]
        public AlloyIntellisenseCache IntellisenseCache
        {
            get;
            private set;
        }

        public override ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new AlloyCompletionSource(textBuffer, this);
        }
    }
}
