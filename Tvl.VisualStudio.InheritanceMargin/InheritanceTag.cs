namespace Tvl.VisualStudio.InheritanceMargin
{
    using Microsoft.VisualStudio.Text.Editor;

    public class InheritanceTag : IGlyphTag
    {
        private readonly InheritanceGlyph _glyph;

        public InheritanceTag(InheritanceGlyph glyph)
        {
            this._glyph = glyph;
        }

        public InheritanceGlyph Glyph
        {
            get
            {
                return _glyph;
            }
        }
    }
}
