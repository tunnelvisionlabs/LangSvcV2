namespace Tvl.VisualStudio.Language.Parsing.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Text;

    [Export(typeof(IBackgroundParserFactoryService))]
    internal class BackgroundParserFactoryService : IBackgroundParserFactoryService
    {
        [ImportMany]
        private IEnumerable<Lazy<IBackgroundParserProvider, IContentTypeMetadata>> BackgroundParserProviders
        {
            get;
            set;
        }

        public IBackgroundParser GetBackgroundParser([NotNull] ITextBuffer buffer)
        {
            Requires.NotNull(buffer, nameof(buffer));

            foreach (var provider in BackgroundParserProviders)
            {
                if (provider.Metadata.ContentTypes.Any(contentType => buffer.ContentType.IsOfType(contentType)))
                {
                    var parser = provider.Value.CreateParser(buffer);
                    if (parser != null)
                        return parser;
                }
            }

            return null;
        }
    }
}
