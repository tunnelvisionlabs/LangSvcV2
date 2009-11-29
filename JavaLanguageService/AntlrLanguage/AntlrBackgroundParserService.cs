namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.ComponentModel.Composition;
    using JavaLanguageService.Panes;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Shell.OutputWindow;

    [Export]
    public sealed class AntlrBackgroundParserService
    {
        [Import]
        internal IOutputWindowService OutputWindowService;

        public AntlrBackgroundParser GetBackgroundParser(ITextBuffer textBuffer)
        {
            Func<AntlrBackgroundParser> creator = () => new AntlrBackgroundParser(textBuffer, OutputWindowService);
            return textBuffer.Properties.GetOrCreateSingletonProperty<AntlrBackgroundParser>(creator);
        }
    }
}
