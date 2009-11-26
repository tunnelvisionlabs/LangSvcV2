namespace JavaLanguageService
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType(Constants.JavaContentType)]
    [TagType(typeof(SquiggleTag))]
    public sealed class JavaErrorTaggerProvider : ITaggerProvider
    {
        [Import]
        internal JavaBackgroundParserService JavaBackgroundParserService;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (typeof(T) == typeof(SquiggleTag))
            {
                Func<JavaErrorTagger> creator = () => new JavaErrorTagger(buffer, JavaBackgroundParserService.GetBackgroundParser(buffer));
                return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(creator);
            }

            return null;
        }
    }
}
