namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;

    [Export(typeof(ITaggerProvider))]
    [ContentType(Antlr4Constants.AntlrContentType)]
    [TagType(typeof(IOutliningRegionTag))]
    internal sealed class Antlr4OutliningTaggerProvider : ITaggerProvider
    {
        private readonly IBackgroundParserFactoryService _backgroundParserFactoryService;

        [ImportingConstructor]
        public Antlr4OutliningTaggerProvider(IBackgroundParserFactoryService backgroundParserFactoryService)
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

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            Func<Antlr4OutliningTagger> creator = () => new Antlr4OutliningTagger(this, buffer);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
