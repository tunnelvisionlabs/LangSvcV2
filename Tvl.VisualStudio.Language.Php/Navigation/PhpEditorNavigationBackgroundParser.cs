namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing4;
    using Tvl.VisualStudio.Language.Php.Parser;
    using Tvl.VisualStudio.Shell.OutputWindow.Interfaces;

    using BackgroundParser = Tvl.VisualStudio.Language.Parsing.BackgroundParser;
    using ErrorListener = Tvl.VisualStudio.Language.Php.Outlining.PhpOutliningBackgroundParser.ErrorListener;
    using ParseErrorEventArgs = Tvl.VisualStudio.Language.Parsing.ParseErrorEventArgs;
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
            var lexer = new PhpLexer(input);
            lexer.TokenFactory = new SnapshotTokenFactory(lexer);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PhpParser(tokens);
            parser.BuildParseTree = true;

            List<ParseErrorEventArgs> errors = new List<ParseErrorEventArgs>();
            parser.AddErrorListener(new ErrorListener(filename, errors, outputWindow));
            var result = parser.compileUnit();

            NavigationTreesListener listener = new NavigationTreesListener();
            ParseTreeWalker.Default.Walk(listener, result);

            OnParseComplete(new PhpEditorNavigationParseResultEventArgs(snapshot, errors, stopwatch.Elapsed, tokens.GetTokens(), result, listener.NavigationTrees));
        }

        protected class NavigationTreesListener : PhpParserBaseListener
        {
            private readonly List<ParserRuleContext> _navigationTrees =
                new List<ParserRuleContext>();

            public ReadOnlyCollection<ParserRuleContext> NavigationTrees
            {
                get
                {
                    return _navigationTrees.AsReadOnly();
                }
            }

            [RuleDependency(typeof(PhpParser), PhpParser.RULE_classOrInterfaceDefinition, 5, Dependents.Parents)]
            public override void EnterClassOrInterfaceDefinition(PhpParser.ClassOrInterfaceDefinitionContext context)
            {
                _navigationTrees.Add(context);
            }

            [RuleDependency(typeof(PhpParser), PhpParser.RULE_functionDefinition, 5, Dependents.Parents)]
            public override void EnterFunctionDefinition(PhpParser.FunctionDefinitionContext context)
            {
                _navigationTrees.Add(context);
            }
        }
    }
}
