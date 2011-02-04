namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using global::Antlr3;
    using global::Antlr3.Tool;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;

    public class AntlrBackgroundParser : BackgroundParser
    {
        public AntlrBackgroundParser(ITextBuffer textBuffer, IOutputWindowService outputWindowService)
            : base(textBuffer)
        {
            this.OutputWindowService = outputWindowService;
        }

        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
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
            var outputWindow = OutputWindowService.TryGetPane(AntlrConstants.AntlrIntellisenseOutputWindow);
            try
            {
                var snapshot = TextBuffer.CurrentSnapshot;
                var input = new SnapshotCharStream(snapshot);
                var lexer = new AntlrErrorProvidingLexer(input);
                var tokens = new AntlrParserTokenStream(lexer);
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
                };

                AntlrTool.ToolPathRoot = typeof(AntlrTool).Assembly.Location;
                ErrorManager.SetErrorListener(new AntlrErrorProvidingParser.ErrorListener());
                Grammar g = new Grammar();
                var result = parser.grammar_(g);
                OnParseComplete(new AntlrParseResultEventArgs(snapshot, errors, tokens.GetTokens(), result));
            }
            catch (Exception e)
            {
                try
                {
                    if (outputWindow != null)
                    {
                        outputWindow.WriteLine(e.Message);
                    }
                }
                catch
                {
                }
            }
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
            }

            RuleSpans = rules;
        }
    }
}
