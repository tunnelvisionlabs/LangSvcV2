namespace Tvl.VisualStudio.Language.Intellisense
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text.Editor;
    using Keyboard = System.Windows.Input.Keyboard;
    using SnapshotSpan = Microsoft.VisualStudio.Text.SnapshotSpan;

    public class SnapshotSpanNavigateToTarget : INavigateToTarget
    {
        private readonly ITextView _textView;
        private readonly SnapshotSpan _snapshotSpan;

        public SnapshotSpanNavigateToTarget([NotNull] ITextView textView, SnapshotSpan snapshotSpan)
        {
            Requires.NotNull(textView, nameof(textView));

            this._textView = textView;
            this._snapshotSpan = snapshotSpan;
        }

        [NotNull]
        public ITextView TextView
        {
            get
            {
                return _textView;
            }
        }

        public SnapshotSpan SnapshotSpan
        {
            get
            {
                return _snapshotSpan;
            }
        }

        public void NavigateTo()
        {
            IWpfTextView wpfTextView = TextView as IWpfTextView;
            if (wpfTextView != null)
            {
                wpfTextView.VisualElement.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextView.Caret.MoveTo(SnapshotSpan.Start);
                    TextView.Selection.Select(SnapshotSpan, false);
                    TextView.ViewScroller.EnsureSpanVisible(SnapshotSpan, EnsureSpanVisibleOptions.ShowStart);
                    Keyboard.Focus(wpfTextView.VisualElement);
                }), null);
            }
        }
    }
}
