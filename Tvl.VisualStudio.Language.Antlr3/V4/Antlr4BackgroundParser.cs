namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using BackgroundParser = Tvl.VisualStudio.Language.Parsing.BackgroundParser;
    using Contract = System.Diagnostics.Contracts.Contract;
    using ParseResultEventArgs = Parsing.ParseResultEventArgs;

    public class Antlr4BackgroundParser : BackgroundParser
    {
        public Antlr4BackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Contract.Requires(textBuffer != null);
            Contract.Requires(taskScheduler != null);
            Contract.Requires(textDocumentFactoryService != null);
            Contract.Requires(outputWindowService != null);
        }

        protected override void ReParseImpl()
        {
            var snapshot = TextBuffer.CurrentSnapshot;
            OnParseComplete(new ParseResultEventArgs(snapshot));
        }
    }
}
