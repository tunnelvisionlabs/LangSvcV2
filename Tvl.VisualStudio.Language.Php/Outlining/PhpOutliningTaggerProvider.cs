namespace Tvl.VisualStudio.Language.Php.Outlining
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell.OutputWindow.Interfaces;

    [Export(typeof(ITaggerProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    [TagType(typeof(IOutliningRegionTag))]
    public sealed class PhpOutliningTaggerProvider : ITaggerProvider
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
            Func<PhpOutliningTagger> creator = () => new PhpOutliningTagger(buffer, this);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
