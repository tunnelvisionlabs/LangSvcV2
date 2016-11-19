namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using IBackgroundParserFactoryService = Parsing.IBackgroundParserFactoryService;

    [Export(typeof(ITaggerProvider))]
    [ContentType(StringTemplateConstants.StringTemplateContentType)]
    [TagType(typeof(IClassificationTag))]
    internal sealed class StringTemplateSemanticTaggerProvider : ITaggerProvider
    {
        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        [Import]
        public IClassificationTypeRegistryService ClassificationTypeRegistryService
        {
            get;
            private set;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            Func<StringTemplateSemanticTagger> creator = () => new StringTemplateSemanticTagger(buffer, BackgroundParserFactoryService.GetBackgroundParser(buffer), ClassificationTypeRegistryService);
            return buffer.Properties.GetOrCreateSingletonProperty(creator) as ITagger<T>;
        }
    }
}
