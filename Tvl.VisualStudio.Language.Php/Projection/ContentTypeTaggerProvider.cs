namespace Tvl.VisualStudio.Language.Php.Projection
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;

    [Export(typeof(ITaggerProvider))]
    // we are tagging the language name which is the automatically-assigned content type of the disk buffer
    [ContentType(PhpConstants.PhpLanguageName)]
    [TagType(typeof(ContentTypeTag))]
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
            if (buffer.Properties.GetProperty(typeof(PhpProjectionBuffer)) == null)
                return null;

            return (ITagger<T>)new ContentTypeTagger(buffer, ContentTypeRegistryService);
        }
    }
}
