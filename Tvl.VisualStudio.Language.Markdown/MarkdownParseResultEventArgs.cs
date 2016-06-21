namespace Tvl.VisualStudio.Language.Markdown
{
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    public class MarkdownParseResultEventArgs : ParseResultEventArgs
    {
        public MarkdownParseResultEventArgs(ITextSnapshot snapshot, string htmlText)
            : base(snapshot)
        {
            this.HtmlText = htmlText ?? string.Empty;
        }

        public string HtmlText
        {
            get;
            private set;
        }
    }
}
