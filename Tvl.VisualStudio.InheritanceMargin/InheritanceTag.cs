namespace Tvl.VisualStudio.InheritanceMargin
{
    using Microsoft.VisualStudio.Text.Editor;

    public class InheritanceTag : IGlyphTag
    {
        private readonly InheritanceGlyph _glyph;
        private readonly string _tooltip;

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
    }
}
