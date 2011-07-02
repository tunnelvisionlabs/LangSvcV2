namespace Tvl.VisualStudio.Language.Parsing
{
    using Path = System.IO.Path;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using Timer = System.Timers.Timer;
    using System.Diagnostics;

    public abstract class BackgroundParser : IBackgroundParser, IDisposable
    {
        private readonly ITextBuffer _textBuffer;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private readonly IOutputWindowService _outputWindowService;
        private readonly string _outputWindowName;
        private readonly Timer _timer;

        private DateTimeOffset _lastEdit;
        private bool _dirty;
        private int _parsing;

        public event EventHandler<ParseResultEventArgs> ParseComplete;

        [Obsolete]
        public BackgroundParser(ITextBuffer textBuffer)
        {
        }

        public BackgroundParser(ITextBuffer textBuffer, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : this(textBuffer, textDocumentFactoryService, outputWindowService, PredefinedOutputWindowPanes.TvlIntellisense)
        {
        }

        public BackgroundParser(ITextBuffer textBuffer, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService, string outputPaneName)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");

            this._textBuffer = textBuffer;
            this._textDocumentFactoryService = textDocumentFactoryService;
            this._outputWindowService = outputWindowService;
            this._outputWindowName = outputPaneName;

            this._textBuffer.PostChanged += TextBufferPostChanged;

            this._dirty = true;
            this._timer = new Timer(1500);
            this._timer.Elapsed += OnParseTimerElapsed;
            this._lastEdit = DateTimeOffset.MinValue;
            this._timer.Start();
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                Contract.Ensures(Contract.Result<ITextBuffer>() != null);

                return _textBuffer;
            }
        }

        public bool Disposed
        {
            get;
            private set;
        }

        public bool IsDisposing
        {
            get;
            private set;
        }

        public void Dispose()
        {
            try
            {
                IsDisposing = false;
                Dispose(true);
            }
            finally
            {
                IsDisposing = false;
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public void RequestParse(bool forceReparse)
        {
            TryReparse(forceReparse);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected abstract void ReParseImpl();

        protected virtual void OnParseComplete(ParseResultEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(e != null, "e");

            var t = ParseComplete;
            if (t != null)
                t(this, e);
        }

        private void MarkDirty()
        {
            this._dirty = true;
            this._lastEdit = DateTimeOffset.Now;
        }

        private void TextBufferPostChanged(object sender, EventArgs e)
        {
            MarkDirty();
        }

        private void OnParseTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TryReparse(_dirty);
        }

        private void TryReparse(bool forceReparse)
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
            try
            {
                _dirty = false;

                IOutputWindowPane outputWindow = null;
                if (_outputWindowService != null && !string.IsNullOrEmpty(_outputWindowName))
                    outputWindow = _outputWindowService.TryGetPane(_outputWindowName);

                Stopwatch stopwatch = Stopwatch.StartNew();

                string message = "{0}: Background parse {1} in {2}ms. {3}";

                string filename = "<Unknown File>";
                ITextDocument textDocument;
                if (_textDocumentFactoryService.TryGetTextDocument(TextBuffer, out textDocument))
                {
                    filename = textDocument.FilePath;
                    if (filename != null)
                        filename = filename.Substring(filename.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) + 1);
                }

                try
                {
                    ReParseImpl();

                    try
                    {
                        if (outputWindow != null)
                        {
                            long time = stopwatch.ElapsedMilliseconds;
                            outputWindow.WriteLine(string.Format(message, filename, "succeeded", time, string.Empty));
                        }
                    }
                    catch (Exception e3)
                    {
                        if (ErrorHandler.IsCriticalException(e3))
                            throw;
                    }
                }
                catch (Exception e2)
                {
                    if (ErrorHandler.IsCriticalException(e2))
                        throw;

                    try
                    {
                        if (outputWindow != null)
                        {
                            long time = stopwatch.ElapsedMilliseconds;
                            outputWindow.WriteLine(string.Format(message, filename, "failed", time, e2.Message));
                        }
                    }
                    catch (Exception e3)
                    {
                        if (ErrorHandler.IsCriticalException(e3))
                            throw;
                    }
                }

            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;
            }
        }
    }
}
