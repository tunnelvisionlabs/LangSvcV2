namespace Tvl.VisualStudio.Language.Parsing
{
    using System;
    using System.Threading;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    public abstract class BackgroundParser : IBackgroundParser, IDisposable
    {
        private System.Timers.Timer _timer;
        private DateTimeOffset _lastEdit;
        private bool _dirty;
        private int _parsing;

        public event EventHandler<ParseResultEventArgs> ParseComplete;

        public BackgroundParser(ITextBuffer textBuffer)
        {
            this.TextBuffer = textBuffer;
            this.TextBuffer.PostChanged += TextBufferPostChanged;

            this._dirty = true;
            this._timer = new System.Timers.Timer(2000);
            this._timer.Elapsed += OnParseTimerElapsed;
            this._lastEdit = DateTimeOffset.MinValue;
            this._timer.Start();
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
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

        protected virtual void Dispose(bool disposing)
        {
        }

        protected abstract void ReParseImpl();

        protected virtual void OnParseComplete(ParseResultEventArgs e)
        {
            var t = ParseComplete;
            if (t != null)
                t(this, e);
        }

        private void TextBufferPostChanged(object sender, EventArgs e)
        {
            this._dirty = true;
        }

        private void OnParseTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
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
                ReParseImpl();
            }
            catch
            {
            }
        }
    }
}
