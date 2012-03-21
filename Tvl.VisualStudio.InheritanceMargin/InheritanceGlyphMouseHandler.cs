namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Formatting;
    using Microsoft.VisualStudio.Text.Tagging;

    internal class InheritanceGlyphMouseHandler : MouseProcessorBase
    {
        private readonly InheritanceGlyphMouseHandlerProvider _provider;
        private readonly IWpfTextViewHost _textViewHost;
        private readonly IWpfTextViewMargin _margin;
        private readonly ITagAggregator<InheritanceTag> _glyphTagAggregator;
        private readonly Popup _popup;

        private Point _clickLocation;
        private bool _lastLeftButtonWasDoubleClick;
        private DispatcherTimer _mouseHoverTimer;
        private ITextViewLine _lastHoverPosition;
        private ITextViewLine _currentlyHoveringLine;

        public InheritanceGlyphMouseHandler(InheritanceGlyphMouseHandlerProvider provider, IWpfTextViewHost textViewHost, IWpfTextViewMargin margin)
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");
            Contract.Requires<ArgumentNullException>(textViewHost != null, "textViewHost");
            Contract.Requires<ArgumentNullException>(margin != null, "margin");

            _provider = provider;
            _textViewHost = textViewHost;
            _margin = margin;
            _glyphTagAggregator = provider.ViewTagAggregatorFactoryService.CreateTagAggregator<InheritanceTag>(textViewHost.TextView);
            _popup = new Popup()
                {
                    IsOpen = false,
                    Visibility = Visibility.Hidden
                };

            _lastLeftButtonWasDoubleClick = true;
            _textViewHost.Closed += (sender, e) => _glyphTagAggregator.Dispose();
        }

        public override void PostprocessMouseEnter(MouseEventArgs e)
        {
            EnableToolTips();
        }

        public override void PostprocessMouseLeave(MouseEventArgs e)
        {
            DisableToolTips();
        }

        public override void PostprocessMouseRightButtonUp(MouseButtonEventArgs e)
        {
            Point mouseLocationInTextView = GetMouseLocationInTextView(e);
            ITextViewLine textViewLine = GetTextViewLine(mouseLocationInTextView.Y);
            if (textViewLine != null)
            {
                var tags = GetInheritanceGlyphTagsStartingOnLine(textViewLine);
                var firstTag = tags.FirstOrDefault();
                if (firstTag != null)
                {
                    FrameworkElement glyphElement = firstTag.MarginGlyph;
                    if (glyphElement != null)
                    {
                        Action action = () => firstTag.ShowContextMenu(e);
                        glyphElement.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
                    }
                }
            }
        }

        public override void PostprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _clickLocation = GetMouseLocationInTextView(e);
            _lastLeftButtonWasDoubleClick = e.ClickCount == 2;
            if (!_lastLeftButtonWasDoubleClick)
                HandleDragStart(_clickLocation);
        }

        public override void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Point mouseLocationInTextView = GetMouseLocationInTextView(e);
            if (HandleDragEnd(mouseLocationInTextView))
            {
                e.Handled = true;
            }
            else if (!_lastLeftButtonWasDoubleClick)
            {
                if (GetTextViewLine(mouseLocationInTextView.Y) != GetTextViewLine(_clickLocation.Y))
                    e.Handled = false;
                else
                    e.Handled = HandleMarkerClick(e);
            }
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            Point mouseLocationInTextView = GetMouseLocationInTextView(e);
            if (!HandleDragOver(mouseLocationInTextView))
            {
                ITextViewLine textViewLine = GetTextViewLine(mouseLocationInTextView.Y);
                if (_mouseHoverTimer != null)
                {
                    if (textViewLine != _currentlyHoveringLine)
                    {
                        _currentlyHoveringLine = null;
                        HideToolTip();
                    }

                    _mouseHoverTimer.Start();
                }
            }
        }

        private IEnumerable<InheritanceTag> GetInheritanceGlyphTagsStartingOnLine(ITextViewLine textViewLine)
        {
            ITextBuffer visualBuffer = _textViewHost.TextView.TextViewModel.VisualBuffer;
            ITextBuffer textBuffer = _textViewHost.TextView.TextBuffer;
            foreach (IMappingTagSpan<IGlyphTag> iteratorVariable2 in _glyphTagAggregator.GetTags(textViewLine.ExtentAsMappingSpan))
            {
                InheritanceTag tag = iteratorVariable2.Tag as InheritanceTag;
                if (tag != null)
                {
                    SnapshotPoint? point = iteratorVariable2.Span.Start.GetPoint(visualBuffer, PositionAffinity.Predecessor);
                    SnapshotPoint? iteratorVariable5 = iteratorVariable2.Span.Start.GetPoint(textBuffer, PositionAffinity.Predecessor);
                    if (point.HasValue && iteratorVariable5.HasValue && iteratorVariable5.Value >= textViewLine.Start && iteratorVariable5.Value <= textViewLine.End)
                        yield return tag;
                }
            }
        }

        private Point GetMouseLocationInTextView(MouseEventArgs e)
        {
            IWpfTextView textView = _textViewHost.TextView;
            Point position = e.GetPosition(textView.VisualElement);
            position.Y += textView.ViewportTop;
            position.X += textView.ViewportLeft;
            return position;
        }

        private ITextViewLine GetTextViewLine(double y)
        {
            IWpfTextView textView = this._textViewHost.TextView;
            ITextViewLine textViewLineContainingYCoordinate = textView.TextViewLines.GetTextViewLineContainingYCoordinate(y);
            if (textViewLineContainingYCoordinate == null)
                textViewLineContainingYCoordinate = (y <= textView.TextViewLines[0].Top) ? textView.TextViewLines.FirstVisibleLine : textView.TextViewLines.LastVisibleLine;

            return textViewLineContainingYCoordinate;
        }

        private void EnableToolTips()
        {
            if (_mouseHoverTimer == null)
                _mouseHoverTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(150), DispatcherPriority.Normal, HandleHoverTimer, _margin.VisualElement.Dispatcher);

            _mouseHoverTimer.Start();
        }

        private void DisableToolTips()
        {
            if (_mouseHoverTimer != null)
                _mouseHoverTimer.Stop();

            HideToolTip();
            _lastHoverPosition = null;
        }

        private void HideToolTip()
        {
            _popup.Child = null;
            _popup.IsOpen = false;
            _popup.Visibility = Visibility.Hidden;
        }

        private bool HandleMarkerClick(MouseButtonEventArgs e)
        {
            return false;
        }

        private void HandleDragStart(Point _clickLocation)
        {
        }

        private bool HandleDragEnd(Point mouseLocationInTextView)
        {
            return false;
        }

        private bool HandleDragOver(Point mouseLocationInTextView)
        {
            return false;
        }

        private void HandleHoverTimer(object sender, EventArgs e)
        {
            if (!_textViewHost.IsClosed && Mouse.LeftButton != MouseButtonState.Pressed)
                HoverAtPoint(Mouse.GetPosition(_margin.VisualElement));
        }

        private void HoverAtPoint(Point point)
        {
            if (_mouseHoverTimer != null && _mouseHoverTimer.IsEnabled && _margin.Enabled)
            {
                ITextViewLine textViewLineContainingYCoordinate = _textViewHost.TextView.TextViewLines.GetTextViewLineContainingYCoordinate(point.Y + _textViewHost.TextView.ViewportTop);
                if (textViewLineContainingYCoordinate != this._lastHoverPosition)
                {
                    this._lastHoverPosition = textViewLineContainingYCoordinate;
                    if (textViewLineContainingYCoordinate != null)
                    {
                        string str = null;
                        foreach (InheritanceTag tag in GetInheritanceGlyphTagsStartingOnLine(textViewLineContainingYCoordinate))
                        {
                            if (!string.IsNullOrEmpty(tag.ToolTip))
                                str = tag.ToolTip;
                        }

                        if (!string.IsNullOrEmpty(str))
                        {
                            this._popup.Child = null;
                            TextBlock block = new TextBlock
                            {
                                Text = str,
                                Name = "InheritanceGlyphToolTip"
                            };

                            Border border = new Border
                            {
                                Padding = new Thickness(1.0),
                                BorderThickness = new Thickness(1.0),
                                Child = block
                            };

                            block.SetResourceReference(TextBlock.ForegroundProperty, "VsBrush.ScreenTipText");
                            border.SetResourceReference(Border.BorderBrushProperty, "VsBrush.ScreenTipBorder");
                            border.SetResourceReference(Border.BackgroundProperty, "VsBrush.ScreenTipBackground");
                            _popup.Child = border;
                            _popup.Placement = PlacementMode.Relative;
                            _popup.PlacementTarget = _margin.VisualElement;
                            _popup.HorizontalOffset = 0.0;
                            _popup.VerticalOffset = textViewLineContainingYCoordinate.Bottom - _textViewHost.TextView.ViewportTop;
                            _popup.IsOpen = true;
                            _popup.Visibility = Visibility.Visible;
                            _currentlyHoveringLine = textViewLineContainingYCoordinate;
                        }
                    }
                }
            }
        }
    }
}
