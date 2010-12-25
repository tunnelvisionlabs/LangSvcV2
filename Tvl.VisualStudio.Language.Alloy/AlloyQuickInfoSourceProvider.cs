namespace Tvl.VisualStudio.Language.Alloy
{
    using IQuickInfoSource = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSource;
    using IQuickInfoSourceProvider = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSourceProvider;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;

    internal class AlloyQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new AlloyQuickInfoSource(textBuffer);
        }
    }
}
