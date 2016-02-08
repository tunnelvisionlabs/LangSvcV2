namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Tagging;

    [Export(typeof(ITaggerProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    [TagType(typeof(IErrorTag))]
    public sealed class AntlrErrorTaggerProvider : ITaggerProvider
    {
        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (typeof(T) == typeof(IErrorTag))
            {
                Func<BackgroundParserErrorTagger> creator = () => new BackgroundParserErrorTagger(buffer, BackgroundParserFactoryService.GetBackgroundParser(buffer));
                return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(creator);
            }

            return null;
        }
    }
}
