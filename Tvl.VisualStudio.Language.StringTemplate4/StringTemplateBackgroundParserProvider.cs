namespace Tvl.VisualStudio.Language.StringTemplate4
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
    [ContentType(StringTemplateConstants.StringTemplateContentType)]
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
            Func<StringTemplateBackgroundParser> creator = () => new StringTemplateBackgroundParser(textBuffer, BackgroundIntelliSenseTaskScheduler, TextDocumentFactoryService, OutputWindowService);
            return textBuffer.Properties.GetOrCreateSingletonProperty<StringTemplateBackgroundParser>(creator);
        }
    }
}
