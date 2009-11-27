namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    [TagType(typeof(SquiggleTag))]
    public sealed class AntlrErrorTaggerProvider : ITaggerProvider
    {
        [Import]
        internal AntlrBackgroundParserService AntlrBackgroundParserService;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (typeof(T) == typeof(SquiggleTag))
            {
                Func<AntlrErrorTagger> creator = () => new AntlrErrorTagger(buffer, AntlrBackgroundParserService.GetBackgroundParser(buffer));
                return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(creator);
            }

            return null;
        }
    }
}
