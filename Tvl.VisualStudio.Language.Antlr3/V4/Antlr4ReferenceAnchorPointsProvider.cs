namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    [Export]
    internal class Antlr4ReferenceAnchorPointsProvider
    {
        private readonly IBackgroundParserFactoryService _backgroundParserFactoryService;

        [ImportingConstructor]
        public Antlr4ReferenceAnchorPointsProvider(IBackgroundParserFactoryService backgroundParserFactoryService)
        {
            _backgroundParserFactoryService = backgroundParserFactoryService;
        }

        public IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get
            {
                return _backgroundParserFactoryService;
            }
        }

        public Antlr4ReferenceAnchorPoints GetReferenceAnchorPoints(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new Antlr4ReferenceAnchorPoints(this, textBuffer));
        }
    }
}
