namespace Tvl.VisualStudio.MouseFastScroll
{
    using ITextView = Microsoft.VisualStudio.Text.Editor.ITextView;
    using Keyboard = System.Windows.Input.Keyboard;
    using ModifierKeys = System.Windows.Input.ModifierKeys;
    using MouseProcessorBase = Microsoft.VisualStudio.Text.Editor.MouseProcessorBase;
    using MouseWheelEventArgs = System.Windows.Input.MouseWheelEventArgs;
    using ScrollDirection = Microsoft.VisualStudio.Text.Editor.ScrollDirection;

    internal class FastScrollProcessor : MouseProcessorBase
    {
        public FastScrollProcessor(ITextView textView)
        {
            TextView = textView;
        }

        private ITextView TextView
        {
            get;
            set;
        }

        public override void PreprocessMouseWheel(MouseWheelEventArgs e)
        {
            var scroller = TextView.ViewScroller;
            if (scroller != null && Keyboard.Modifiers == ModifierKeys.Control)
            {
                scroller.ScrollViewportVerticallyByPage(e.Delta < 0 ? ScrollDirection.Down : ScrollDirection.Up);
                e.Handled = true;
            }
        }
    }
}
