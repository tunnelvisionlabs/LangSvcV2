namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;

    [Export(typeof(IQuickInfoSourceProvider))]
    [Order]
    [ContentType(Antlr4Constants.AntlrContentType)]
    [Name("Antlr4QuickInfoSource")]
    public sealed class Antlr4QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory
        {
            get;
            private set;
        }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(textBuffer) as Antlr4BackgroundParser;
            return new Antlr4QuickInfoSource(textBuffer, backgroundParser, AggregatorFactory.CreateTagAggregator<ClassificationTag>(textBuffer));
        }
    }
}
