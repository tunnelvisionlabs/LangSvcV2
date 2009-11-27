namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;
    using System.Threading;
    using Antlr.Runtime;
    using JavaLanguageService.Panes;
    using Antlr3.Grammars;
    using Antlr3.Tool;
    using Antlr3;

    public class AntlrBackgroundParser : IDisposable
    {
        private System.Timers.Timer _timer;
        private DateTimeOffset _lastEdit;
        private bool _dirty;
        private int _parsing;

        public event EventHandler<ParseResultEventArgs> ParseComplete;

        public AntlrBackgroundParser(ITextBuffer textBuffer, IOutputWindowService outputWindowService)
        {
            this.TextBuffer = textBuffer;
            this.TextBuffer.PostChanged += TextBufferPostChanged;
            this.OutputWindowService = outputWindowService;

            this._dirty = true;
            this._timer = new System.Timers.Timer(2000);
            this._timer.Elapsed += ParseTimerElapsed;
            this._lastEdit = DateTimeOffset.MinValue;
            this._timer.Start();
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
        }

        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        public void Dispose()
        {
        }

        private void TextBufferPostChanged(object sender, EventArgs e)
        {
            this._dirty = true;
        }

        private void ParseTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_dirty)
                return;

            if (DateTimeOffset.Now - _lastEdit < TimeSpan.FromSeconds(2))
                return;

            if (Interlocked.CompareExchange(ref _parsing, 1, 0) == 0)
            {
                try
                {
                    Action action = ReParse;
                    action.BeginInvoke((asyncResult) => _parsing = 0, null);
                }
                catch
                {
                    _parsing = 0;
                    throw;
                }
            }
        }

        private void ReParse()
        {
            _dirty = false;
            var outputWindow = OutputWindowService.TryGetPane(Constants.AntlrIntellisenseOutputWindow);
            try
            {
                SnapshotCharStream input = new SnapshotCharStream(TextBuffer.CurrentSnapshot);
                var lexer = new AntlrErrorProvidingLexer(input);
                AntlrParserTokenStream tokens = new AntlrParserTokenStream(lexer);
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
                OnParseComplete(new ParseResultEventArgs(result, errors));
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

        private void OnParseComplete(ParseResultEventArgs e)
        {
            var t = ParseComplete;
            if (t != null)
                t(this, e);
        }
    }
}
