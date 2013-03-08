namespace Tvl.VisualStudio.Language.Php.Projection
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;

    [Export(typeof(ITaggerProvider))]
    [ContentType("projection")]
    [TagType(typeof(ContentTypeTag))]
    [Order]
    internal sealed class ContentTypeTaggerProvider : ITaggerProvider
    {
        [Import]
        public IContentTypeRegistryService ContentTypeRegistryService
        {
            get;
            private set;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return (ITagger<T>)new ContentTypeTagger(buffer, ContentTypeRegistryService);
        }
    }
}
