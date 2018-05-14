namespace Tvl.VisualStudio.Language.Go
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Antlr.Runtime;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using Stopwatch = System.Diagnostics.Stopwatch;

    public class GoBackgroundParser : BackgroundParser
    {
        public GoBackgroundParser([NotNull] ITextBuffer textBuffer, [NotNull] TaskScheduler taskScheduler, [NotNull] ITextDocumentFactoryService textDocumentFactoryService, [NotNull] IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Debug.Assert(textBuffer != null);
            Debug.Assert(taskScheduler != null);
            Debug.Assert(textDocumentFactoryService != null);
            Debug.Assert(outputWindowService != null);
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                var snapshot = TextBuffer.CurrentSnapshot;
                SnapshotCharStream input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
                GoLexer lexer = new GoLexer(input);
                GoSemicolonInsertionTokenSource tokenSource = new GoSemicolonInsertionTokenSource(lexer);
                CommonTokenStream tokens = new CommonTokenStream(tokenSource);
                GoParser parser = new GoParser(tokens);
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
            catch (Exception e) when (!ErrorHandler.IsCriticalException(e))
            {
                try
                {
                    if (outputWindow != null)
                        outputWindow.WriteLine(e.Message);
                }
                catch (Exception ex2) when (!ErrorHandler.IsCriticalException(ex2))
                {
                }
            }
        }
    }
}
