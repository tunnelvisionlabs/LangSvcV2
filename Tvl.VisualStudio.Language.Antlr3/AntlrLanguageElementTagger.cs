namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using global::Antlr3.Tool;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using Tvl.VisualStudio.Text.Tagging;
    using StringReader = System.IO.StringReader;

    internal sealed class AntlrLanguageElementTagger : ITagger<ILanguageElementTag>
    {
        private System.Timers.Timer _timer;
        private DateTimeOffset _lastEdit;
        private bool _dirty;
        private int _parsing;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public AntlrLanguageElementTagger(ITextBuffer textBuffer, IBackgroundParser backgroundParser, IOutputWindowService outputWindowService)
        {
            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;
            this.OutputWindowService = outputWindowService;
            this.BackgroundParser.ParseComplete += HandleBackgroundParseComplete;

            this._dirty = false;
            this._timer = new System.Timers.Timer(2000);
            this._timer.Elapsed += ParseTimerElapsed;
            this._lastEdit = DateTimeOffset.MinValue;
            this._timer.Start();

            this.BackgroundParser.RequestParse(false);
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
        }

        public IBackgroundParser BackgroundParser
        {
            get;
            private set;
        }

        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        public IEnumerable<ITagSpan<ILanguageElementTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            yield break;
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }

        private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            if (e.Errors.Count == 0)
            {
                this._dirty = true;
                this._lastEdit = DateTimeOffset.Now;
            }
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
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);
            try
            {
                Grammar g = new Grammar();
                ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;
                g.ParseAndBuildAST(new StringReader(snapshot.GetText()));
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
