namespace Tvl.VisualStudio.Language.Go
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;

    [Export(typeof(IBackgroundParserProvider))]
    [ContentType(GoConstants.GoContentType)]
    public sealed class GoBackgroundParserProvider : IBackgroundParserProvider
    {
        [Import]
        private IOutputWindowService OutputWindowService
        {
            get;
            set;
        }

        public IBackgroundParser CreateParser(ITextBuffer textBuffer)
        {
            Func<GoBackgroundParser> creator = () => new GoBackgroundParser(textBuffer, OutputWindowService);
            return textBuffer.Properties.GetOrCreateSingletonProperty<GoBackgroundParser>(creator);
        }
    }
}
