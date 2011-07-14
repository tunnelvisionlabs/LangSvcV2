namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using IOutputWindowService = Tvl.VisualStudio.Shell.OutputWindow.IOutputWindowService;
    using IQuickInfoSource = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSource;
    using IQuickInfoSourceProvider = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSourceProvider;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;

    [Export(typeof(IQuickInfoSourceProvider))]
    [Order]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Name("AlloyQuickInfoSource")]
    internal class AlloyQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        public AlloyIntellisenseCache IntellisenseCache
        {
            get;
            private set;
        }

        [Import]
        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new AlloyQuickInfoSource(textBuffer, this);
        }
    }
}
