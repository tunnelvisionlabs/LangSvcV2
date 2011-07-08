namespace Tvl.VisualStudio.Language.Java
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    [Export(typeof(ITaggerProvider))]
    [ContentType(Constants.JavaContentType)]
    [TagType(typeof(IOutliningRegionTag))]
    public sealed class OutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            Func<OutliningTagger> creator = () => new OutliningTagger(buffer, BackgroundParserFactoryService.GetBackgroundParser(buffer));
            return buffer.Properties.GetOrCreateSingletonProperty<OutliningTagger>(creator) as ITagger<T>;
        }
    }
}
