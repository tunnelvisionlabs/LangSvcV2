namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using Tvl.VisualStudio.Text.Tagging;

    [Export(typeof(ITaggerProvider))]
    [ContentType(Antlr4Constants.AntlrContentType)]
    [TagType(typeof(ILanguageElementTag))]
    public sealed class Antlr4LanguageElementTaggerProvider : ITaggerProvider
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
                        return new Antlr4LanguageElementTagger(buffer, backgroundParser, OutputWindowService);
                    };
                return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(creator);
            }

            return null;
        }
    }
}
