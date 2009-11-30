namespace Tvl.VisualStudio.Text.Navigation.Implementation
{
    using System.ComponentModel.Composition;
    using JavaLanguageService.Text.Language;
    using JavaLanguageService.Text.Tagging;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IWpfTextViewMarginProvider))]
    [MarginContainer(PredefinedMarginNames.Top)]
    [ContentType("text")]
    [Order]
    [Name("Editor Navigation Margin")]
    [TextViewRole(PredefinedTextViewRoles.Structured)]
    public sealed class EditorNavigationMarginProvider : IWpfTextViewMarginProvider
    {
        [Import]
        public IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService;

        [Import]
        public ILanguageElementManagerService LanguageElementManagerService;

        [Import]
        public IGlyphService GlyphService;

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            var tagAggregator = BufferTagAggregatorFactoryService.CreateTagAggregator<ILanguageElementTag>(wpfTextViewHost.TextView.TextBuffer);
            //var manager = LanguageElementManagerService.GetLanguageElementManager(wpfTextViewHost.TextView);
            //if (manager == null)
            //    return null;

            return new EditorNavigationMargin(wpfTextViewHost.TextView, tagAggregator, GlyphService);
        }
    }
}
