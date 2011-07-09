namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using PhpOutliningLexer = Tvl.VisualStudio.Language.Php.Outlining.PhpOutliningLexer;
    using PhpOutliningParser = Tvl.VisualStudio.Language.Php.Outlining.PhpOutliningParser;
    using Stopwatch = System.Diagnostics.Stopwatch;

    internal class PhpEditorNavigationBackgroundParser : BackgroundParser
    {
        private readonly ITextBuffer _textBuffer;
        private readonly IOutputWindowService _outputWindowService;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;

        private PhpEditorNavigationBackgroundParser(ITextBuffer textBuffer, IOutputWindowService outputWindowService, ITextDocumentFactoryService textDocumentFactoryService)
            : base(textBuffer, textDocumentFactoryService, outputWindowService)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (outputWindowService == null)
                throw new ArgumentNullException("outputWindowService");
            if (textDocumentFactoryService == null)
                throw new ArgumentNullException("textDocumentFactoryService");

            _textBuffer = textBuffer;
            _outputWindowService = outputWindowService;
            _textDocumentFactoryService = textDocumentFactoryService;
        }

        internal static PhpEditorNavigationBackgroundParser CreateParser(ITextBuffer textBuffer, IOutputWindowService outputWindowService, ITextDocumentFactoryService textDocumentFactoryService)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (outputWindowService == null)
                throw new ArgumentNullException("outputWindowService");
            if (textDocumentFactoryService == null)
                throw new ArgumentNullException("textDocumentFactoryService");

            return new PhpEditorNavigationBackgroundParser(textBuffer, outputWindowService, textDocumentFactoryService);
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
