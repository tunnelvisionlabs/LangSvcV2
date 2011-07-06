namespace Tvl.VisualStudio.Language.Php.Outlining
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Language.Php.Classification;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using Stopwatch = System.Diagnostics.Stopwatch;

    public class PhpOutliningBackgroundParser : BackgroundParser
    {
        private readonly ITextBuffer _textBuffer;
        private readonly IOutputWindowService _outputWindowService;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;

        private PhpOutliningBackgroundParser(ITextBuffer textBuffer, IOutputWindowService outputWindowService, ITextDocumentFactoryService textDocumentFactoryService)
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

        public IOutputWindowService OutputWindowService
        {
            get
            {
                return _outputWindowService;
            }
        }

        internal static PhpOutliningBackgroundParser CreateParser(ITextBuffer textBuffer, IOutputWindowService outputWindowService, ITextDocumentFactoryService textDocumentFactoryService)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (outputWindowService == null)
                throw new ArgumentNullException("outputWindowService");
            if (textDocumentFactoryService == null)
                throw new ArgumentNullException("textDocumentFactoryService");

            return new PhpOutliningBackgroundParser(textBuffer, outputWindowService, textDocumentFactoryService);
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);

            Stopwatch stopwatch = Stopwatch.StartNew();

            var snapshot = TextBuffer.CurrentSnapshot;
            var input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            var lexer = new PhpClassifierLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PhpOutliningParser(tokens);

            List<ParseErrorEventArgs> errors = new List<ParseErrorEventArgs>();
            parser.ParseError += (sender, e) =>
            {
                errors.Add(e);

                string message = e.Message;
                if (message.Length > 100)
                    message = message.Substring(0, 100) + " ...";

                if (outputWindow != null)
                    outputWindow.WriteLine(message);
            };

            var result = parser.compileUnit();
            OnParseComplete(new AntlrParseResultEventArgs(snapshot, errors, stopwatch.Elapsed, tokens.GetTokens(), result));
        }
    }
}
