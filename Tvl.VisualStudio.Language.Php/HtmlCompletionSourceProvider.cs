namespace Tvl.VisualStudio.Language.Php
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ICompletionSourceProvider)), ContentType("HTML"), Order, Name("HtmlCompletionProvider")]
    internal class HtmlCompletionSourceProvider : ICompletionSourceProvider
    {
        internal readonly IGlyphService _glyphService;

        [ImportingConstructor]
        public HtmlCompletionSourceProvider(IGlyphService glyphService)
        {
            _glyphService = glyphService;
        }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new HtmlCompletionSource(this, textBuffer);
        }
    }
}
