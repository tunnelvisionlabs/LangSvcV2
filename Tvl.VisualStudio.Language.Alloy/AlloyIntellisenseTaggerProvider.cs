namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    [TagType(typeof(AlloyIntellisenseTag))]
    internal class AlloyIntellisenseTaggerProvider : ITaggerProvider
    {
        [Import]
        public IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService
        {
            get;
            private set;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            Func<AlloyIntellisenseTagger> creator = () => new AlloyIntellisenseTagger(buffer, this);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
