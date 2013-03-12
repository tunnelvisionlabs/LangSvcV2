namespace Tvl.VisualStudio.Language.Php.Projection
{
    using System;
    using System.Diagnostics.Contracts;
    using IContentType = Microsoft.VisualStudio.Utilities.IContentType;
    using ITag = Microsoft.VisualStudio.Text.Tagging.ITag;

    public class ContentTypeTag : ITag
    {
        private readonly IContentType _contentType;
        private readonly RegionType _regionType;

        public ContentTypeTag(IContentType contentType, RegionType regionType)
        {
            Contract.Requires<ArgumentNullException>(contentType != null, "contentType");
            Contract.Requires<ArgumentException>(regionType == RegionType.Begin || regionType == RegionType.End);

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
