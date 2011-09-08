namespace Tvl.VisualStudio.Text.Navigation.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using IVsDropdownBarClient = Microsoft.VisualStudio.TextManager.Interop.IVsDropdownBarClient;
    using IWpfTextView = Microsoft.VisualStudio.Text.Editor.IWpfTextView;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Editor;
    using Tvl.VisualStudio.Shell.Extensions;

    [Export(typeof(IEditorNavigationDropdownBarFactoryService))]
    public class EditorNavigationDropdownBarFactoryService : IEditorNavigationDropdownBarFactoryService
    {
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

        public IEditorNavigationDropdownBarClient CreateEditorNavigationDropdownBar(IVsCodeWindow codeWindow, IVsEditorAdaptersFactoryService editorAdaptersFactory)
        {
            // a code window can only be associated with a single buffer, so the primary view will get us the correct information
            IVsTextView primaryViewAdapter = codeWindow.GetPrimaryView();
            IWpfTextView textView = editorAdaptersFactory.GetWpfTextView(primaryViewAdapter);

            var providers = NavigationSourceProviders.Where(provider => provider.Metadata.ContentTypes.Any(contentType => textView.TextBuffer.ContentType.IsOfType(contentType)));

            var sources =
                providers
                .Select(provider => provider.Value.TryCreateEditorNavigationSource(textView.TextBuffer))
                .Where(source => source != null)
                .ToArray();

            return new EditorNavigationDropdownBar(codeWindow, editorAdaptersFactory, sources, EditorNavigationTypeRegistryService);
        }
    }
}
