namespace Tvl.VisualStudio.Language.Markdown
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell;
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

        [Import]
        private ITextDocumentFactoryService TextDocumentFactoryService
        {
            get;
            set;
        }

        [Import(PredefinedTaskSchedulers.BackgroundIntelliSense)]
        public TaskScheduler BackgroundIntelliSenseTaskScheduler
        {
            get;
            private set;
        }

        public IBackgroundParser CreateParser(ITextBuffer textBuffer)
        {
            Func<MarkdownBackgroundParser> creator = () => new MarkdownBackgroundParser(textBuffer, BackgroundIntelliSenseTaskScheduler, TextDocumentFactoryService, OutputWindowService);
            return textBuffer.Properties.GetOrCreateSingletonProperty<MarkdownBackgroundParser>(creator);
        }
    }
}
