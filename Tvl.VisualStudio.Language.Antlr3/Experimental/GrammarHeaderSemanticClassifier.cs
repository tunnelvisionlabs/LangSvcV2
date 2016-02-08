namespace Tvl.VisualStudio.Language.Antlr3.Experimental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing.Collections;

    /// <summary>
    /// The sole purpose of this classifier is identifying grammar headers:
    ///   * lexer grammar X
    ///   * parser grammar X
    ///   * tree grammar X
    ///   * grammar X
    /// </summary>
    public class GrammarHeaderSemanticClassifier
    {
        private readonly ITextBuffer _textBuffer;
        private readonly IntervalSet _dirtySpans;

        public GrammarHeaderSemanticClassifier()
        {
        }

        protected virtual void SubscribeEvents()
        {
            _textBuffer.ChangedLowPriority += HandleTextBufferChangedLowPriority;
            _textBuffer.ChangedHighPriority += HandleTextBufferChangedHighPriority;
        }

        protected virtual void UnsubscribeEvents()
        {
            _textBuffer.ChangedLowPriority -= HandleTextBufferChangedLowPriority;
            _textBuffer.ChangedHighPriority -= HandleTextBufferChangedHighPriority;
        }

        protected virtual void HandleTextBufferChangedLowPriority(object sender, TextContentChangedEventArgs e)
        {
            if (e.After == _textBuffer.CurrentSnapshot)
            {
                //if (_firstChangedLine.HasValue && _lastChangedLine.HasValue)
                //{
                //    int startLine = _firstChangedLine.Value;
                //    int endLine = Math.Min(_lastChangedLine.Value, e.After.LineCount - 1);

                //    _firstChangedLine = null;
                //    _lastChangedLine = null;

                //    ForceReclassifyLines(startLine, endLine);
                //}
            }
        }

        protected virtual void HandleTextBufferChangedHighPriority(object sender, TextContentChangedEventArgs e)
        {
            foreach (ITextChange change in e.Changes)
            {
                //// process the old position
                //_dirtySpans.Remove(new Interval(change.OldPosition, change.OldLength));

                //// process the addition of the new position

                //int lineNumberFromPosition = e.After.GetLineNumberFromPosition(change.NewPosition);
                //int num2 = e.After.GetLineNumberFromPosition(change.NewEnd);
                //if (change.LineCountDelta < 0)
                //{
                //    _lineStates.RemoveRange(lineNumberFromPosition, Math.Abs(change.LineCountDelta));
                //}
                //else if (change.LineCountDelta > 0)
                //{
                //    TState endLineState = _lineStates[lineNumberFromPosition].EndLineState;
                //    LineStateInfo element = new LineStateInfo(endLineState);
                //    _lineStates.InsertRange(lineNumberFromPosition, Enumerable.Repeat(element, change.LineCountDelta));
                //}

                //if (_lastDirtyLine.HasValue && _lastDirtyLine.Value > lineNumberFromPosition)
                //{
                //    _lastDirtyLine += change.LineCountDelta;
                //}

                //if (_lastChangedLine.HasValue && _lastChangedLine.Value > lineNumberFromPosition)
                //{
                //    _lastChangedLine += change.LineCountDelta;
                //}

                //for (int i = lineNumberFromPosition; i <= num2; i++)
                //{
                //    TState num5 = _lineStates[i].EndLineState;
                //    LineStateInfo info2 = new LineStateInfo(num5, true);
                //    _lineStates[i] = info2;
                //}

                //_firstChangedLine = _firstChangedLine.HasValue ? Math.Min(_firstChangedLine.Value, lineNumberFromPosition) : lineNumberFromPosition;
                //_lastChangedLine = _lastChangedLine.HasValue ? Math.Max(_lastChangedLine.Value, num2) : num2;
            }
        }
    }
}
