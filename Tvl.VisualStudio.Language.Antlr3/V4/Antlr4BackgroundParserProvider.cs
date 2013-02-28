namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell.OutputWindow.Interfaces;

    [Export(typeof(IBackgroundParserProvider))]
    [ContentType(Antlr4Constants.AntlrContentType)]
    public sealed class Antlr4BackgroundParserProvider : IBackgroundParserProvider
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
            Func<Antlr4BackgroundParser> creator = () => new Antlr4BackgroundParser(textBuffer, BackgroundIntelliSenseTaskScheduler, TextDocumentFactoryService, OutputWindowService);
            return textBuffer.Properties.GetOrCreateSingletonProperty<Antlr4BackgroundParser>(creator);
        }
    }
}
