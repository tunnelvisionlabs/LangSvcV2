namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;

    [Export(typeof(IBackgroundParserProvider))]
    [ContentType(MarkdownConstants.MarkdownContentType)]
    public sealed class GoBackgroundParserProvider : IBackgroundParserProvider
    {
        [Import]
        private IOutputWindowService OutputWindowService
        {
            get;
            set;
        }

        public IBackgroundParser CreateParser(ITextBuffer textBuffer)
        {
            Func<MarkdownBackgroundParser> creator = () => new MarkdownBackgroundParser(textBuffer, OutputWindowService);
            return textBuffer.Properties.GetOrCreateSingletonProperty<MarkdownBackgroundParser>(creator);
        }
    }
}
