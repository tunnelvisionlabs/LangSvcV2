namespace Tvl.VisualStudio.Language.Go.Experimental
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell;
    using Tvl.VisualStudio.OutputWindow.Interfaces;

    [Export(typeof(ITaggerProvider))]
    [ContentType(GoConstants.GoContentType)]
    [TagType(typeof(IClassificationTag))]
    public sealed class GoSymbolTaggerProvider : ITaggerProvider
    {
        [Import]
        public IClassificationTypeRegistryService ClassificationTypeRegistryService
        {
            get;
            private set;
        }

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

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            Func<GoSymbolTagger> creator = () => new GoSymbolTagger(buffer, ClassificationTypeRegistryService, BackgroundIntelliSenseTaskScheduler, TextDocumentFactoryService, OutputWindowService);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
