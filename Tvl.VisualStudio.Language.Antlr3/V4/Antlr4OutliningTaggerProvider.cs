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
    public sealed class Antlr4OutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            Func<Antlr4OutliningTagger> creator = () => new Antlr4OutliningTagger(buffer, (Antlr4BackgroundParser)BackgroundParserFactoryService.GetBackgroundParser(buffer), this);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
