namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using Contract = System.Diagnostics.Contracts.Contract;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using Stopwatch = System.Diagnostics.Stopwatch;

    public class Antlr4BackgroundParser : BackgroundParser
    {
        private static bool _initialized;

        public Antlr4BackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Contract.Requires(textBuffer != null);
            Contract.Requires(taskScheduler != null);
            Contract.Requires(textDocumentFactoryService != null);
            Contract.Requires(outputWindowService != null);

            if (!_initialized)
            {
                try
                {
#if false
                    // have to create an instance of the tool to make sure the error manager gets initialized
                    new AntlrTool();
#endif
                }
                catch (Exception e)
                {
                    if (ErrorHandler.IsCriticalException(e))
                        throw;
                }


                _initialized = true;
            }
        }

        public AntlrParseResultEventArgs PreviousParseResult
        {
            get;
            private set;
        }

        public Dictionary<string, KeyValuePair<ITrackingSpan, ITrackingPoint>> RuleSpans
        {
            get;
            private set;
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);

            Stopwatch stopwatch = Stopwatch.StartNew();

            var snapshot = TextBuffer.CurrentSnapshot;
            var input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
#if false
            var lexer = new AntlrErrorProvidingLexer(input);
            var tokens = new Antlr4ParserTokenStream(lexer);
            var parser = new AntlrErrorProvidingParser(tokens);

            lexer.Parser = parser;
            tokens.Parser = parser;

            List<ParseErrorEventArgs> errors = new List<ParseErrorEventArgs>();
            parser.ParseError += (sender, e) =>
            {
                errors.Add(e);

                string message = e.Message;
                if (message.Length > 100)
                    message = message.Substring(0, 100) + " ...";

                if (outputWindow != null)
                    outputWindow.WriteLine(message);

                if (errors.Count > 100)
                    throw new OperationCanceledException();
            };

            AntlrTool.ToolPathRoot = typeof(AntlrTool).Assembly.Location;
            ErrorManager.SetErrorListener(new AntlrErrorProvidingParser.ErrorListener());
            Grammar g = new Grammar();
            var result = parser.grammar_(g);
            OnParseComplete(new AntlrParseResultEventArgs(snapshot, errors, stopwatch.Elapsed, tokens.GetTokens(), result));
#endif
        }

        protected override void OnParseComplete(ParseResultEventArgs e)
        {
            PreviousParseResult = e as AntlrParseResultEventArgs;
            ExtractRuleSpans();
            base.OnParseComplete(e);
        }

        private void ExtractRuleSpans()
        {
            Dictionary<string, KeyValuePair<ITrackingSpan, ITrackingPoint>> rules = new Dictionary<string, KeyValuePair<ITrackingSpan, ITrackingPoint>>();
            var antlrParseResultArgs = PreviousParseResult;
            if (antlrParseResultArgs != null)
            {
#if false
                IAstRuleReturnScope resultArgs = antlrParseResultArgs.Result as IAstRuleReturnScope;
                var result = resultArgs != null ? resultArgs.Tree as CommonTree : null;
                if (result != null)
                {
                    foreach (CommonTree child in result.Children)
                    {
                        if (child == null || string.IsNullOrEmpty(child.Text))
                            continue;

                        if (child.Text == "rule" && child.ChildCount > 0)
                        {
                            var ruleName = child.GetChild(0).Text;
                            if (string.IsNullOrEmpty(ruleName))
                                continue;

                            if (ruleName == "Tokens")
                                continue;

                            IToken startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                            IToken stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                            ITrackingSpan trackingSpan = antlrParseResultArgs.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeNegative);
                            ITrackingPoint trackingPoint = antlrParseResultArgs.Snapshot.CreateTrackingPoint(((CommonTree)child.GetChild(0)).Token.StartIndex, PointTrackingMode.Negative);
                            rules[ruleName] = new KeyValuePair<ITrackingSpan, ITrackingPoint>(trackingSpan, trackingPoint);
                        }
                        else if (child.Text.StartsWith("tokens"))
                        {
                            foreach (CommonTree tokenChild in child.Children)
                            {
                                if (tokenChild.Text == "=" && tokenChild.ChildCount == 2)
                                {
                                    var ruleName = tokenChild.GetChild(0).Text;
                                    if (string.IsNullOrEmpty(ruleName))
                                        continue;

                                    IToken startToken = antlrParseResultArgs.Tokens[tokenChild.TokenStartIndex];
                                    IToken stopToken = antlrParseResultArgs.Tokens[tokenChild.TokenStopIndex];
                                    Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                    ITrackingSpan trackingSpan = antlrParseResultArgs.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeNegative);
                                    ITrackingPoint trackingPoint = antlrParseResultArgs.Snapshot.CreateTrackingPoint(((CommonTree)tokenChild.GetChild(0)).Token.StartIndex, PointTrackingMode.Negative);
                                    rules[ruleName] = new KeyValuePair<ITrackingSpan, ITrackingPoint>(trackingSpan, trackingPoint);
                                }
                                else if (tokenChild.ChildCount == 0)
                                {
                                    var ruleName = tokenChild.Text;
                                    if (string.IsNullOrEmpty(ruleName))
                                        continue;

                                    IToken startToken = antlrParseResultArgs.Tokens[tokenChild.TokenStartIndex];
                                    IToken stopToken = antlrParseResultArgs.Tokens[tokenChild.TokenStopIndex];
                                    Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                    ITrackingSpan trackingSpan = antlrParseResultArgs.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeNegative);
                                    ITrackingPoint trackingPoint = antlrParseResultArgs.Snapshot.CreateTrackingPoint(tokenChild.Token.StartIndex, PointTrackingMode.Negative);
                                    rules[ruleName] = new KeyValuePair<ITrackingSpan, ITrackingPoint>(trackingSpan, trackingPoint);
                                }
                            }
                        }
                    }
                }
#endif
            }

            RuleSpans = rules;
        }
    }
}
