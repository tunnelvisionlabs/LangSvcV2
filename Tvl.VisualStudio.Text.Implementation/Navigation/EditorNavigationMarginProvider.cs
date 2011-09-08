namespace Tvl.VisualStudio.Text.Navigation.Implementation
{
    using IVsDropdownBarManager = Microsoft.VisualStudio.TextManager.Interop.IVsDropdownBarManager;
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
    using Microsoft.VisualStudio.Editor;
    using Tvl.VisualStudio.Shell.Extensions;
    using IVsTextView = Microsoft.VisualStudio.TextManager.Interop.IVsTextView;
    using IVsTextBuffer = Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer;

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

        [Import]
        private IVsEditorAdaptersFactoryService EditorAdaptersFactoryService
        {
            get;
            set;
        }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            /* If a language service is registered for this content type, then the CodeWindowManager will
             * support the standard editor navigation margin.
             */
            IVsTextBuffer textBufferAdapter = EditorAdaptersFactoryService.GetBufferAdapter(wpfTextViewHost.TextView.TextBuffer);
            if (textBufferAdapter != null)
            {
                Guid? languageService = textBufferAdapter.GetLanguageServiceID();
                if (languageService.HasValue)
                    return null;
            }

            //var viewAdapter = EditorAdaptersFactoryService.GetViewAdapter(wpfTextViewHost.TextView);
            //var codeWindow = viewAdapter.GetCodeWindow();
            //var dropdownBarManager = codeWindow as IVsDropdownBarManager;
            //if (dropdownBarManager != null && dropdownBarManager.GetDropdownBarClient() != null)
            //    return null;

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
