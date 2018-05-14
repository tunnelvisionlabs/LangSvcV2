namespace Tvl.VisualStudio.Language.Php.Projection
{
    using JetBrains.Annotations;
    using IContentType = Microsoft.VisualStudio.Utilities.IContentType;
    using ITag = Microsoft.VisualStudio.Text.Tagging.ITag;

    public class ContentTypeTag : ITag
    {
        private readonly IContentType _contentType;
        private readonly RegionType _regionType;

        public ContentTypeTag([NotNull] IContentType contentType, RegionType regionType)
        {
            Requires.NotNull(contentType, nameof(contentType));
            Requires.Argument(regionType == RegionType.Begin || regionType == RegionType.End, nameof(regionType), $"Unexpected region type: {regionType}");

            _contentType = contentType;
            _regionType = regionType;
        }

        public IContentType ContentType
        {
            get
            {
                return _contentType;
            }
        }

        public RegionType RegionType
        {
            get
            {
                return _regionType;
            }
        }
    }
}
