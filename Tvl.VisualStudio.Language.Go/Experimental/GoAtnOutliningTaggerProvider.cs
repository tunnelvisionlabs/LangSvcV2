namespace Tvl.VisualStudio.Language.Go.Experimental
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell;
    using IOutputWindowService = Tvl.VisualStudio.OutputWindow.Interfaces.IOutputWindowService;

    [Export(typeof(ITaggerProvider))]
    [ContentType(GoConstants.GoContentType)]
    [TagType(typeof(IOutliningRegionTag))]
    public sealed class GoAtnOutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService
        {
            get;
            private set;
        }

        [Import(PredefinedTaskSchedulers.BackgroundIntelliSense)]
        public TaskScheduler BackgroundIntelliSenseTaskScheduler
        {
            get;
            private set;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            Func<GoAtnOutliningTagger> creator = () => new GoAtnOutliningTagger(buffer, BackgroundIntelliSenseTaskScheduler, TextDocumentFactoryService, OutputWindowService);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
