namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using CommonTokenStream = Antlr.Runtime.CommonTokenStream;
    using Contract = System.Diagnostics.Contracts.Contract;
    using OperationCanceledException = System.OperationCanceledException;
    using Stopwatch = System.Diagnostics.Stopwatch;
    using TaskScheduler = System.Threading.Tasks.TaskScheduler;

    public class AlloyBackgroundParser : BackgroundParser
    {
        public AlloyBackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Contract.Requires(textBuffer != null);
            Contract.Requires(taskScheduler != null);
            Contract.Requires(textDocumentFactoryService != null);
            Contract.Requires(outputWindowService != null);
        }

        public override string Name
        {
            get
            {
                return "Default";
            }
        }

        public AntlrParseResultEventArgs PreviousParseResult
        {
            get;
            private set;
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);

            Stopwatch stopwatch = Stopwatch.StartNew();

            var snapshot = TextBuffer.CurrentSnapshot;
            SnapshotCharStream input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            AlloyLexer lexer = new AlloyLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            AlloyParser parser = new AlloyParser(tokens);
            List<ParseErrorEventArgs> errors = new List<ParseErrorEventArgs>();
            parser.ParseError += (sender, e) =>
                {
                    errors.Add(e);

                    string message = e.Message;

                    ITextDocument document;
                    if (TextBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document) && document != null)
                    {
                        string fileName = document.FilePath;
                        var line = snapshot.GetLineFromPosition(e.Span.Start);
                        message = string.Format("{0}({1},{2}): {3}", fileName, line.LineNumber + 1, e.Span.Start - line.Start.Position + 1, message);
                    }

                    if (message.Length > 100)
                        message = message.Substring(0, 100) + " ...";

                    if (outputWindow != null)
                        outputWindow.WriteLine(message);

                    if (errors.Count > 100)
                        throw new OperationCanceledException();
                };

            var result = parser.compilationUnit();
            OnParseComplete(new AntlrParseResultEventArgs(snapshot, errors, stopwatch.Elapsed, tokens.GetTokens(), result));
        }

        protected override void OnParseComplete(ParseResultEventArgs e)
        {
            PreviousParseResult = e as AntlrParseResultEventArgs;
            base.OnParseComplete(e);
        }
    }
}
