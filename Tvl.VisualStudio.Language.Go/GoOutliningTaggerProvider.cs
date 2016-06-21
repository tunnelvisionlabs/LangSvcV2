namespace Tvl.VisualStudio.Language.Go
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;

#if false
    [Export(typeof(ITaggerProvider))]
#endif
    [ContentType(GoConstants.GoContentType)]
    [TagType(typeof(IOutliningRegionTag))]
    public sealed class GoOutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            Func<GoOutliningTagger> creator = () => new GoOutliningTagger(buffer, BackgroundParserFactoryService.GetBackgroundParser(buffer), this);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
