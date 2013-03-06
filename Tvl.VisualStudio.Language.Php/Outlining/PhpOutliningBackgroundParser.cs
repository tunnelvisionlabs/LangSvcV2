namespace Tvl.VisualStudio.Language.Php.Outlining
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Php.Parser;
    using Tvl.VisualStudio.Shell.OutputWindow.Interfaces;
    using BackgroundParser = Tvl.VisualStudio.Language.Parsing.BackgroundParser;
    using BaseErrorListener = Antlr4.Runtime.BaseErrorListener;
    using CommonTokenStream = Antlr4.Runtime.CommonTokenStream;
    using ParseErrorEventArgs = Tvl.VisualStudio.Language.Parsing.ParseErrorEventArgs;
    using SnapshotCharStream = Tvl.VisualStudio.Language.Parsing4.SnapshotCharStream;
    using SnapshotTokenFactory = Tvl.VisualStudio.Language.Parsing4.SnapshotTokenFactory;
    using Stopwatch = System.Diagnostics.Stopwatch;

    public class PhpOutliningBackgroundParser : BackgroundParser
    {
        private PhpOutliningBackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, IOutputWindowService outputWindowService, ITextDocumentFactoryService textDocumentFactoryService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Contract.Requires(textBuffer != null);
            Contract.Requires(taskScheduler != null);
            Contract.Requires(outputWindowService != null);
            Contract.Requires(textDocumentFactoryService != null);
        }

        internal static PhpOutliningBackgroundParser CreateParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, IOutputWindowService outputWindowService, ITextDocumentFactoryService textDocumentFactoryService)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");
            Contract.Requires<ArgumentNullException>(taskScheduler != null, "taskScheduler");
            Contract.Requires<ArgumentNullException>(outputWindowService != null, "outputWindowService");
            Contract.Requires<ArgumentNullException>(textDocumentFactoryService != null, "textDocumentFactoryService");

            return new PhpOutliningBackgroundParser(textBuffer, taskScheduler, outputWindowService, textDocumentFactoryService);
        }

        [RuleDependency(typeof(PhpParser), PhpParser.RULE_compileUnit, 0, Dependents.Parents)]
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
            var lexer = new PhpLexer(input);
            lexer.TokenFactory = new SnapshotTokenFactory(lexer);
            var tokens = new CommonTokenStream(lexer);

            var parser = new PhpParser(tokens);
            parser.BuildParseTree = true;

            List<ParseErrorEventArgs> errors = new List<ParseErrorEventArgs>();
            parser.AddErrorListener(new ErrorListener(filename, errors, outputWindow));
            var result = parser.compileUnit();

            OutliningTreesListener listener = new OutliningTreesListener();
            ParseTreeWalker.Default.Walk(listener, result);

            OnParseComplete(new PhpOutliningParseResultEventArgs(snapshot, errors, stopwatch.Elapsed, tokens.GetTokens(), result, listener.OutliningTrees));
        }

        public class ErrorListener : BaseErrorListener
        {
            private readonly string _fileName;
            private readonly List<ParseErrorEventArgs> _errors;
            private readonly IOutputWindowPane _outputWindow;

            public ErrorListener(string fileName, List<ParseErrorEventArgs> errors, IOutputWindowPane outputWindow)
            {
                Contract.Requires<ArgumentNullException>(errors != null, "errors");

                _fileName = fileName;
                _errors = errors;
                _outputWindow = outputWindow;
            }

            public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                Span span = new Span();
                if (offendingSymbol != null)
                    span = Span.FromBounds(offendingSymbol.StartIndex, offendingSymbol.StopIndex + 1);

                _errors.Add(new ParseErrorEventArgs(msg, span));

                if (_outputWindow != null)
                {
                    if (msg.Length > 100)
                        msg = msg.Substring(0, 100) + " ...";

                    _outputWindow.WriteLine(string.Format("{0}({1}:{2}): {3}", _fileName ?? recognizer.InputStream.SourceName, line, charPositionInLine, msg));
                }

                if (_errors.Count > 100)
                    throw new OperationCanceledException();
            }
        }

        protected class OutliningTreesListener : PhpParserBaseListener
        {
            private readonly List<ParserRuleContext> _outliningTrees =
                new List<ParserRuleContext>();

            public ReadOnlyCollection<ParserRuleContext> OutliningTrees
            {
                get
                {
                    return _outliningTrees.AsReadOnly();
                }
            }

            [RuleDependency(typeof(PhpParser), PhpParser.RULE_code, 0, Dependents.Parents)]
            public override void EnterCode(PhpParser.CodeContext context)
            {
                _outliningTrees.Add(context);
            }

            [RuleDependency(typeof(PhpParser), PhpParser.RULE_classOrInterfaceDefinition, 0, Dependents.Parents)]
            [RuleDependency(typeof(PhpParser), PhpParser.RULE_codeBlock, 0, Dependents.Self)]
            public override void EnterClassOrInterfaceDefinition(PhpParser.ClassOrInterfaceDefinitionContext context)
            {
                var codeBlock = context.codeBlock();
                if (codeBlock != null)
                    _outliningTrees.Add(context.codeBlock());
            }

            [RuleDependency(typeof(PhpParser), PhpParser.RULE_functionDefinition, 0, Dependents.Parents)]
            [RuleDependency(typeof(PhpParser), PhpParser.RULE_codeBlock, 0, Dependents.Self)]
            public override void EnterFunctionDefinition(PhpParser.FunctionDefinitionContext context)
            {
                var codeBlock = context.codeBlock();
                if (codeBlock != null)
                    _outliningTrees.Add(context.codeBlock());
            }
        }
    }
}
