namespace JavaLanguageService.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using JavaLanguageService.Text.Language;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text.Tagging;

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
