namespace JavaLanguageService
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using Tvl.VisualStudio.Language.Parsing;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IBackgroundParserProvider))]
    [ContentType(Constants.JavaContentType)]
    public sealed class JavaBackgroundParserProvider : IBackgroundParserProvider
    {
        [Import]
        internal IOutputWindowService OutputWindowService;

        public IBackgroundParser CreateParser(ITextBuffer textBuffer)
        {
            Func<JavaBackgroundParser> creator = () => new JavaBackgroundParser(textBuffer, OutputWindowService);
            return textBuffer.Properties.GetOrCreateSingletonProperty<JavaBackgroundParser>(creator);
        }
    }
}
