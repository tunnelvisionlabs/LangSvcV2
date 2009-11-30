namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using JavaLanguageService.Panes;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using Tvl.VisualStudio.Text.Tagging;

    [Export(typeof(ITaggerProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    [TagType(typeof(ILanguageElementTag))]
    public sealed class AntlrLanguageElementTaggerProvider : ITaggerProvider
    {
        [Import]
        internal AntlrBackgroundParserService AntlrBackgroundParserService;

        [Import]
        internal IOutputWindowService OutputWindowService;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (typeof(T) == typeof(ILanguageElementTag))
            {
                Func<ITagger<ILanguageElementTag>> creator =
                    () =>
                    {
                        var backgroundParser = AntlrBackgroundParserService.GetBackgroundParser(buffer);
                        return new AntlrLanguageElementTagger(buffer, backgroundParser, OutputWindowService);
                    };
                return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(creator);
            }

            return null;
        }
    }
}
