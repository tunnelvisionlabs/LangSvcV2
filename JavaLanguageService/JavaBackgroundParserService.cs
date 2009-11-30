namespace JavaLanguageService
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Shell.OutputWindow;

    [Export]
    public sealed class JavaBackgroundParserService
    {
        [Import]
        internal IOutputWindowService OutputWindowService;

        public JavaBackgroundParser GetBackgroundParser(ITextBuffer textBuffer)
        {
            Func<JavaBackgroundParser> creator = () => new JavaBackgroundParser(textBuffer, OutputWindowService);
            return textBuffer.Properties.GetOrCreateSingletonProperty<JavaBackgroundParser>(creator);
        }
    }
}
