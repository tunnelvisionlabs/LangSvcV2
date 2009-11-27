namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using JavaLanguageService.Panes;

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
