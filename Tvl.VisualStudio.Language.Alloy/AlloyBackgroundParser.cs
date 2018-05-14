namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.OutputWindow.Interfaces;

    using CommonTokenStream = Antlr.Runtime.CommonTokenStream;
    using OperationCanceledException = System.OperationCanceledException;
    using Stopwatch = System.Diagnostics.Stopwatch;
    using TaskScheduler = System.Threading.Tasks.TaskScheduler;

    public class AlloyBackgroundParser : BackgroundParser
    {
        public AlloyBackgroundParser([NotNull] ITextBuffer textBuffer, [NotNull] TaskScheduler taskScheduler, [NotNull] ITextDocumentFactoryService textDocumentFactoryService, [NotNull] IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Debug.Assert(textBuffer != null);
            Debug.Assert(taskScheduler != null);
            Debug.Assert(textDocumentFactoryService != null);
            Debug.Assert(outputWindowService != null);
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
