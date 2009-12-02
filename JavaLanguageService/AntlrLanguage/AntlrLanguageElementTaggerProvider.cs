namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using Tvl.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;

    [Export(typeof(ITaggerProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    [TagType(typeof(ILanguageElementTag))]
    public sealed class AntlrLanguageElementTaggerProvider : ITaggerProvider
    {
        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        [Import]
        private IOutputWindowService OutputWindowService
        {
            get;
            set;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (typeof(T) == typeof(ILanguageElementTag))
            {
                Func<ITagger<ILanguageElementTag>> creator =
                    () =>
                    {
                        var backgroundParser = BackgroundParserFactoryService.GetBackgroundParser(buffer);
                        return new AntlrLanguageElementTagger(buffer, backgroundParser, OutputWindowService);
                    };
                return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(creator);
            }

            return null;
        }
    }
}
