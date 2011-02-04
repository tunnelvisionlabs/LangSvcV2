namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
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
            base.OnParseComplete(e);
        }
    }
}
