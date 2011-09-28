namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow.Interfaces;
    using PhpOutliningLexer = Tvl.VisualStudio.Language.Php.Outlining.PhpOutliningLexer;
    using Stopwatch = System.Diagnostics.Stopwatch;

    internal class PhpEditorNavigationBackgroundParser : BackgroundParser
    {
        private PhpEditorNavigationBackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, IOutputWindowService outputWindowService, ITextDocumentFactoryService textDocumentFactoryService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Contract.Requires(textBuffer != null);
            Contract.Requires(taskScheduler != null);
            Contract.Requires(outputWindowService != null);
            Contract.Requires(textDocumentFactoryService != null);
        }

        internal static PhpEditorNavigationBackgroundParser CreateParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, IOutputWindowService outputWindowService, ITextDocumentFactoryService textDocumentFactoryService)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");
            Contract.Requires<ArgumentNullException>(taskScheduler != null, "taskScheduler");
            Contract.Requires<ArgumentNullException>(outputWindowService != null, "outputWindowService");
            Contract.Requires<ArgumentNullException>(textDocumentFactoryService != null, "textDocumentFactoryService");

            return new PhpEditorNavigationBackgroundParser(textBuffer, taskScheduler, outputWindowService, textDocumentFactoryService);
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);

            Stopwatch stopwatch = Stopwatch.StartNew();

            string filename = "<Unknown File>";
            ITextDocument textDocument;
            if (TextDocumentFactoryService.TryGetTextDocument(TextBuffer, out textDocument))
                filename = textDocument.FilePath;

            var snapshot = TextBuffer.CurrentSnapshot;
            var input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            var lexer = new PhpOutliningLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PhpEditorNavigationParser(tokens);

            List<ParseErrorEventArgs> errors = new List<ParseErrorEventArgs>();
            parser.ParseError += (sender, e) =>
            {
                errors.Add(e);

                string message = e.Message;
                if (message.Length > 100)
                    message = message.Substring(0, 100) + " ...";

                ITextSnapshotLine startLine = snapshot.GetLineFromPosition(e.Span.Start);
                int line = startLine.LineNumber;
                int column = e.Span.Start - startLine.Start;

                if (outputWindow != null)
                    outputWindow.WriteLine(string.Format("{0}({1}:{2}): {3}", filename, line, column, message));

                if (errors.Count > 100)
                    throw new OperationCanceledException();
            };

            var result = parser.compileUnit();
            OnParseComplete(new PhpEditorNavigationParseResultEventArgs(snapshot, errors, stopwatch.Elapsed, tokens.GetTokens(), result, parser.NavigationTrees));
        }
    }
}
