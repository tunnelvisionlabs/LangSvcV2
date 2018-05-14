namespace Tvl.VisualStudio.Language.Parsing
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using Path = System.IO.Path;
    using Timer = System.Threading.Timer;

    public abstract class BackgroundParser : IBackgroundParser, IDisposable
    {
        private readonly Tvl.WeakReference<ITextBuffer> _textBuffer;
        private readonly TaskScheduler _taskScheduler;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private readonly IOutputWindowService _outputWindowService;
        private readonly string _outputWindowName;
        private readonly Timer _timer;

        private TimeSpan _reparseDelay;
        private DateTimeOffset _lastEdit;
        private bool _dirty;
        private int _parsing;

        public event EventHandler<ParseResultEventArgs> ParseComplete;

        [Obsolete]
        public BackgroundParser([NotNull] ITextBuffer textBuffer, [NotNull] ITextDocumentFactoryService textDocumentFactoryService, [NotNull] IOutputWindowService outputWindowService)
            : this(textBuffer, TaskScheduler.Default, textDocumentFactoryService, outputWindowService, PredefinedOutputWindowPanes.TvlDiagnostics)
        {
            Debug.Assert(textBuffer != null);
            Debug.Assert(textDocumentFactoryService != null);
            Debug.Assert(outputWindowService != null);
        }

        public BackgroundParser([NotNull] ITextBuffer textBuffer, [NotNull] TaskScheduler taskScheduler, [NotNull] ITextDocumentFactoryService textDocumentFactoryService, [NotNull] IOutputWindowService outputWindowService)
            : this(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService, PredefinedOutputWindowPanes.TvlDiagnostics)
        {
            Debug.Assert(textBuffer != null);
            Debug.Assert(taskScheduler != null);
            Debug.Assert(textDocumentFactoryService != null);
            Debug.Assert(outputWindowService != null);
        }

        public BackgroundParser([NotNull] ITextBuffer textBuffer, [NotNull] TaskScheduler taskScheduler, [NotNull] ITextDocumentFactoryService textDocumentFactoryService, [NotNull] IOutputWindowService outputWindowService, string outputPaneName)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));
            Requires.NotNull(taskScheduler, nameof(taskScheduler));
            Requires.NotNull(textDocumentFactoryService, nameof(textDocumentFactoryService));
            Requires.NotNull(outputWindowService, nameof(outputWindowService));

            this._textBuffer = new Tvl.WeakReference<ITextBuffer>(textBuffer);
            this._taskScheduler = taskScheduler;
            this._textDocumentFactoryService = textDocumentFactoryService;
            this._outputWindowService = outputWindowService;
            this._outputWindowName = outputPaneName;

            textBuffer.PostChanged += TextBufferPostChanged;

            this._dirty = true;
            this._reparseDelay = TimeSpan.FromMilliseconds(1500);
            this._timer = new Timer(ParseTimerCallback, null, _reparseDelay, _reparseDelay);
            this._lastEdit = DateTimeOffset.MinValue;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer.Target;
            }
        }

        public ITextDocument TextDocument
        {
            get
            {
                ITextBuffer textBuffer = TextBuffer;
                if (textBuffer == null)
                    return null;

                ITextDocument textDocument;
                if (!TextDocumentFactoryService.TryGetTextDocument(textBuffer, out textDocument))
                    return null;

                return textDocument;
            }
        }

        public bool Disposed
        {
            get;
            private set;
        }

        public TimeSpan ReparseDelay
        {
            get
            {
                return _reparseDelay;
            }

            set
            {
                TimeSpan originalDelay = _reparseDelay;
                try
                {
                    _reparseDelay = value;
                    _timer.Change(value, value);
                }
                catch (ArgumentException)
                {
                    _reparseDelay = originalDelay;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual string Name
        {
            get
            {
                return string.Empty;
            }
        }

        protected ITextDocumentFactoryService TextDocumentFactoryService
        {
            get
            {
                return _textDocumentFactoryService;
            }
        }

        protected IOutputWindowService OutputWindowService
        {
            get
            {
                return _outputWindowService;
            }
        }

        public void RequestParse(bool forceReparse)
        {
            TryReparse(forceReparse);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ITextBuffer textBuffer = TextBuffer;
                if (textBuffer != null)
                    textBuffer.PostChanged -= TextBufferPostChanged;

                _timer.Dispose();
            }

            Disposed = true;
        }

        protected abstract void ReParseImpl();

        protected virtual void OnParseComplete(ParseResultEventArgs e)
        {
            Requires.NotNull(e, nameof(e));

            var t = ParseComplete;
            if (t != null)
                t(this, e);
        }

        protected void MarkDirty(bool resetTimer)
        {
            this._dirty = true;
            this._lastEdit = DateTimeOffset.Now;

            if (resetTimer)
                _timer.Change(_reparseDelay, _reparseDelay);
        }

        private void TextBufferPostChanged(object sender, EventArgs e)
        {
            MarkDirty(true);
        }

        private void ParseTimerCallback(object state)
        {
            if (TextBuffer == null)
            {
                Dispose();
                return;
            }

            TryReparse(_dirty);
        }

        private void TryReparse(bool forceReparse)
        {
            if (!_dirty && !forceReparse)
                return;

            if (Interlocked.CompareExchange(ref _parsing, 1, 0) == 0)
            {
                try
                {
                    Task task = Task.Factory.StartNew(ReParse, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
                    task.ContinueWith(_ => _parsing = 0);
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

                string message = "{0}: Background parse {1}{2} in {3}ms. {4}";
                string name = Name;
                if (!string.IsNullOrEmpty(name))
                    name = "(" + name + ") ";

                string filename = "<Unknown File>";
                ITextDocument textDocument = TextDocument;
                if (textDocument != null)
                {
                    filename = textDocument.FilePath;
                    if (filename != null)
                        filename = filename.Substring(filename.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) + 1);
                }

                try
                {
                    ReParseImpl();

                    if (outputWindow != null)
                    {
                        long time = stopwatch.ElapsedMilliseconds;
                        outputWindow.WriteLine(string.Format(message, filename, name, "succeeded", time, string.Empty));
                    }
                }
                catch (Exception e2) when (!ErrorHandler.IsCriticalException(e2))
                {
                    try
                    {
                        if (outputWindow != null)
                        {
                            long time = stopwatch.ElapsedMilliseconds;
                            outputWindow.WriteLine(string.Format(message, filename, name, "failed", time, e2.Message + e2.StackTrace));
                        }
                    }
                    catch (Exception e3) when (!ErrorHandler.IsCriticalException(e3))
                    {
                    }
                }
            }
            catch (Exception ex) when (!ErrorHandler.IsCriticalException(ex))
            {
            }
        }
    }
}
