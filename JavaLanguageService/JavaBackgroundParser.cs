namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;

    public class JavaBackgroundParser : BackgroundParser
    {
        public JavaBackgroundParser(ITextBuffer textBuffer, IOutputWindowService outputWindowService)
            : base(textBuffer)
        {
            this.OutputWindowService = outputWindowService;
        }

        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(Constants.AntlrIntellisenseOutputWindow);
            try
            {
                var snapshot = TextBuffer.CurrentSnapshot;
                SnapshotCharStream input = new SnapshotCharStream(snapshot);
                Java2Lexer lexer = new Java2Lexer(new JavaUnicodeStream(input));
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                Java2Parser parser = new Java2Parser(tokens);
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

                var result = parser.compilationUnit();
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
    }
}
