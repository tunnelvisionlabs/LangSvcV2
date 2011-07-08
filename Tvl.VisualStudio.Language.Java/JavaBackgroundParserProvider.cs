namespace Tvl.VisualStudio.Language.Java
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;

    [Export(typeof(IBackgroundParserProvider))]
    [ContentType(Constants.JavaContentType)]
    public sealed class JavaBackgroundParserProvider : IBackgroundParserProvider
    {
        [Import]
        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService
        {
            get;
            private set;
        }

        public IBackgroundParser CreateParser(ITextBuffer textBuffer)
        {
            Func<JavaBackgroundParser> creator = () => new JavaBackgroundParser(textBuffer, this);
            return textBuffer.Properties.GetOrCreateSingletonProperty<JavaBackgroundParser>(creator);
        }
    }
}
