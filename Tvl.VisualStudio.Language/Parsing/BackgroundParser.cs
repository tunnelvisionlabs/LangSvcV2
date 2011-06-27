namespace Tvl.VisualStudio.Language.Parsing
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using Microsoft.VisualStudio.Text;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using Timer = System.Timers.Timer;

    public abstract class BackgroundParser : IBackgroundParser, IDisposable
    {
        private readonly ITextBuffer _textBuffer;
        private readonly Timer _timer;

        private DateTimeOffset _lastEdit;
        private bool _dirty;
        private int _parsing;

        public event EventHandler<ParseResultEventArgs> ParseComplete;

        public BackgroundParser(ITextBuffer textBuffer)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");

            this._textBuffer = textBuffer;
            this._textBuffer.PostChanged += TextBufferPostChanged;

            this._dirty = true;
            this._timer = new Timer(1500);
            this._timer.Elapsed += OnParseTimerElapsed;
            this._lastEdit = DateTimeOffset.MinValue;
            this._timer.Start();
            TryReparse();
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

        public void RequestParse()
        {
            if (_parsing == 0 && !_dirty)
                MarkDirty();
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
            TryReparse();
        }

        private void TryReparse()
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
                ReParseImpl();
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;
            }
        }
    }
}
