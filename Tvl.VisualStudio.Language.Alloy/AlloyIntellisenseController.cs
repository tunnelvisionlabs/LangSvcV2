namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Text;

    // see VBIntellisenseController
    internal class AlloyIntellisenseController : IIntellisenseController
    {
        private ITextView _textView;
        private readonly AlloyCompletionControllerProvider _provider;

        public AlloyIntellisenseController(ITextView textView, AlloyCompletionControllerProvider provider)
        {
            this._provider = provider;
            this.TextView = textView;
            _textView.Selection.SelectionChanged += OnSelectionChanged;
        }

        private ITextView TextView
        {
            get
            {
                return _textView;
            }

            set
            {
                lock (this)
                {
                    if (_textView == value)
                        return;

                    if (_textView != null)
                    {
                        _textView.Closed -= OnTextViewClosed;
                        _textView.MouseHover -= OnMouseHover;
                    }

                    _textView = value;
                    if (_textView != null)
                    {
                        _textView.Closed += OnTextViewClosed;
                        _textView.MouseHover += OnMouseHover;
                    }
                }
            }
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void Detach(ITextView textView)
        {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void OnMouseHover(object sender, MouseHoverEventArgs e)
        {
            ICompletionTarget completionTarget = _provider.CompletionTargetMapService.GetCompletionTargetForTextView(TextView);
            if (completionTarget.QuickInfoSession == null)
            {
                ITrackingPoint point = TextView.TextSnapshot.CreateTrackingPoint(e.Position, PointTrackingMode.Negative);
                completionTarget.TriggerQuickInfo(point);
            }
        }

        public void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!TextView.Selection.IsEmpty)
            {
                ICompletionTarget completionTarget = _provider.CompletionTargetMapService.GetCompletionTargetForTextView(TextView);
                if (completionTarget != null)
                    completionTarget.DismissAll();
            }
        }

        public void OnTextViewClosed(object sender, EventArgs e)
        {
            TextView.Selection.SelectionChanged -= OnSelectionChanged;
            Detach(TextView);
        }
    }
}
