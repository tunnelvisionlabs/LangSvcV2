namespace Tvl.VisualStudio.Language.Alloy.Experimental
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
    [ContentType(AlloyConstants.AlloyContentType)]
    [TagType(typeof(IClassificationTag))]
    public sealed class AlloySymbolTaggerProvider : ITaggerProvider
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
            Func<AlloySymbolTagger> creator = () => new AlloySymbolTagger(buffer, ClassificationTypeRegistryService, BackgroundIntelliSenseTaskScheduler, TextDocumentFactoryService, OutputWindowService);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
