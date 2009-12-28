namespace Tvl.VisualStudio.Text.Navigation.Implementation
{
    using Tvl.VisualStudio.Text.Navigation;
    using System.Linq;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Tagging;
    using System;
    using System.Collections.Generic;

    [Export(typeof(IWpfTextViewMarginProvider))]
    [MarginContainer(PredefinedMarginNames.Top)]
    [ContentType("text")]
    [Order]
    [Name("Editor Navigation Margin")]
    [TextViewRole(PredefinedTextViewRoles.Structured)]
    public sealed class EditorNavigationMarginProvider : IWpfTextViewMarginProvider
    {
        //[Import]
        //private IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService
        //{
        //    get;
        //    set;
        //}

        //[Import]
        //private IGlyphService GlyphService
        //{
        //    get;
        //    set;
        //}

        [ImportMany]
        private IEnumerable<Lazy<IEditorNavigationSourceProvider, IEditorNavigationSourceMetadata>> NavigationSourceProviders
        {
            get;
            set;
        }

        [Import]
        private IEditorNavigationTypeRegistryService EditorNavigationTypeRegistryService
        {
            get;
            set;
        }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            var providers = NavigationSourceProviders.Where(provider => provider.Metadata.ContentTypes.Any(contentType => wpfTextViewHost.TextView.TextBuffer.ContentType.IsOfType(contentType)));

            var sources =
                providers
                .Select(provider => provider.Value.TryCreateEditorNavigationSource(wpfTextViewHost.TextView.TextBuffer))
                .Where(source => source != null)
                .ToArray();

            return new EditorNavigationMargin(wpfTextViewHost.TextView, sources, EditorNavigationTypeRegistryService);

            //var tagAggregator = BufferTagAggregatorFactoryService.CreateTagAggregator<ILanguageElementTag>(wpfTextViewHost.TextView.TextBuffer);
            ////var manager = LanguageElementManagerService.GetLanguageElementManager(wpfTextViewHost.TextView);
            ////if (manager == null)
            ////    return null;

            //return new EditorNavigationMargin(wpfTextViewHost.TextView, tagAggregator, GlyphService);
        }
    }
}
