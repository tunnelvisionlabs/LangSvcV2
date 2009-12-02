namespace Tvl.VisualStudio.Text.Navigation.Implementation
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Tagging;

    [Export(typeof(IWpfTextViewMarginProvider))]
    [MarginContainer(PredefinedMarginNames.Top)]
    [ContentType("text")]
    [Order]
    [Name("Editor Navigation Margin")]
    [TextViewRole(PredefinedTextViewRoles.Structured)]
    public sealed class EditorNavigationMarginProvider : IWpfTextViewMarginProvider
    {
        [Import]
        private IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService
        {
            get;
            set;
        }

        //[Import]
        //private ILanguageElementManagerService LanguageElementManagerService{get;set;}

        [Import]
        private IGlyphService GlyphService
        {
            get;
            set;
        }

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
