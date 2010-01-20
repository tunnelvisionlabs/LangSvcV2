namespace Tvl.VisualStudio.Text.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;
    using System.ComponentModel.Composition;

    [Export(typeof(IEditorNavigationSourceAggregatorFactoryService))]
    internal class EditorNavigationSourceAggregatorFactoryService : IEditorNavigationSourceAggregatorFactoryService
    {
        [ImportMany]
        private IEnumerable<Lazy<IEditorNavigationSourceProvider, IEditorNavigationSourceMetadata>> NavigationSourceProviders
        {
            get;
            set;
        }

        public IEditorNavigationSourceAggregator CreateEditorNavigationSourceAggregator(ITextBuffer textBuffer)
        {
            var providers = NavigationSourceProviders.Where(provider => provider.Metadata.ContentTypes.Any(contentType => textBuffer.ContentType.IsOfType(contentType)));

            var sources =
                providers
                .Select(provider => provider.Value.TryCreateEditorNavigationSource(textBuffer))
                .Where(source => source != null)
                .ToArray();

            return new EditorNavigationSourceAggregator(sources);
        }
    }
}
