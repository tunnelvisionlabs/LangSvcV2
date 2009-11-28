namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.ComponentModel.Composition;
    using JavaLanguageService.Text.Language;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    [TagType(typeof(ILanguageElementTag))]
    public sealed class AntlrLanguageElementTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (typeof(T) == typeof(ILanguageElementTag))
            {
                Func<ITagger<ILanguageElementTag>> creator = () => new AntlrLanguageElementTagger(buffer);
                return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(creator);
            }

            return null;
        }
    }
}
