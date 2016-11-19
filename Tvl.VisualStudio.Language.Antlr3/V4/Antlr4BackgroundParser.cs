namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing4;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using BackgroundParser = Tvl.VisualStudio.Language.Parsing.BackgroundParser;
    using Contract = System.Diagnostics.Contracts.Contract;
    using ParseErrorEventArgs = Parsing.ParseErrorEventArgs;

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
            AntlrParseResultEventArgs result = ParseSnapshot(snapshot);
            OnParseComplete(result);
        }

        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_grammarSpec, 0, Dependents.Self)]
        internal static AntlrParseResultEventArgs ParseSnapshot(ITextSnapshot snapshot)
        {
            Stopwatch timer = Stopwatch.StartNew();

            ITokenSource tokenSource = new GrammarLexer(new AntlrInputStream(snapshot.GetText()));
            CommonTokenStream tokenStream = new CommonTokenStream(tokenSource);
            GrammarParser.GrammarSpecContext parseResult;
            GrammarParser parser = new GrammarParser(tokenStream);
            List<ParseErrorEventArgs> errors = new List<ParseErrorEventArgs>();
            try
            {
                parser.Interpreter.PredictionMode = PredictionMode.Sll;
                parser.RemoveErrorListeners();
                parser.BuildParseTree = true;
                parser.ErrorHandler = new BailErrorStrategy();
                parseResult = parser.grammarSpec();
            }
            catch (ParseCanceledException ex) when (ex.InnerException is RecognitionException)
            {
                tokenStream.Reset();
                parser.Interpreter.PredictionMode = PredictionMode.Ll;
                //parser.AddErrorListener(DescriptiveErrorListener.Default);
                parser.SetInputStream(tokenStream);
                parser.ErrorHandler = new DefaultErrorStrategy();
                parseResult = parser.grammarSpec();
            }

            return new AntlrParseResultEventArgs(snapshot, errors, timer.Elapsed, tokenStream.GetTokens(), parseResult);
        }
    }
}
