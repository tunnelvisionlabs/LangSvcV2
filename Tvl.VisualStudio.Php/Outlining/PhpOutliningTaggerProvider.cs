namespace Tvl.VisualStudio.Language.Php.Outlining
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell.OutputWindow;

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

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            Func<PhpOutliningTagger> creator = () => new PhpOutliningTagger(buffer, this);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
