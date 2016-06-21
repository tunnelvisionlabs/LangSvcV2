namespace Tvl.VisualStudio.Language.Antlr3
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;

    [Export(typeof(IQuickInfoSourceProvider))]
    [Order]
    [ContentType(AntlrConstants.AntlrContentType)]
    [Name("AntlrQuickInfoSource")]
    public sealed class AntlrQuickInfoSourceProvider : IQuickInfoSourceProvider
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
            var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(textBuffer) as AntlrBackgroundParser;
            return new AntlrQuickInfoSource(textBuffer, backgroundParser, AggregatorFactory.CreateTagAggregator<ClassificationTag>(textBuffer));
        }
    }
}
