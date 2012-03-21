namespace Tvl.VisualStudio.InheritanceMargin
{
    using Microsoft.VisualStudio.Text.Editor;
    using CommandRouter = Tvl.VisualStudio.InheritanceMargin.CommandTranslation.CommandRouter;
    using FrameworkElement = System.Windows.FrameworkElement;
    using MouseEventArgs = System.Windows.Input.MouseEventArgs;

    public class InheritanceTag : IGlyphTag
    {
        private readonly InheritanceGlyph _glyph;
        private readonly string _tooltip;
        private FrameworkElement _marginGlyph;

        public InheritanceTag(InheritanceGlyph glyph, string tooltip)
        {
            this._glyph = glyph;
            this._tooltip = tooltip;
        }

        public InheritanceGlyph Glyph
        {
            get
            {
                return _glyph;
            }
        }

        public string ToolTip
        {
            get
            {
                return _tooltip;
            }
        }

        public FrameworkElement MarginGlyph
        {
            get
            {
                return _marginGlyph;
            }

            internal set
            {
                _marginGlyph = value;
            }
        }

        public void ShowContextMenu(MouseEventArgs e)
        {
            CommandRouter.DisplayContextMenu(InheritanceMarginConstants.guidInheritanceMarginCommandSet, InheritanceMarginConstants.menuInheritanceTargets, _marginGlyph);
        }
    }
}
