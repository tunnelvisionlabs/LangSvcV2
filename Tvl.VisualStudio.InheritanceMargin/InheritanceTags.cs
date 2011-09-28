namespace Tvl.VisualStudio.InheritanceMargin
{
    public static class InheritanceTags
    {
        public static readonly InheritanceTag None = new InheritanceTag(InheritanceGlyph.None);
        public static readonly InheritanceTag HasImplementations = new InheritanceTag(InheritanceGlyph.HasImplementations);
        public static readonly InheritanceTag Implements = new InheritanceTag(InheritanceGlyph.Implements);
        public static readonly InheritanceTag ImplementsAndHasImplementations = new InheritanceTag(InheritanceGlyph.ImplementsAndHasImplementations);
        public static readonly InheritanceTag ImplementsAndOverridden = new InheritanceTag(InheritanceGlyph.ImplementsAndOverridden);
        public static readonly InheritanceTag Overridden = new InheritanceTag(InheritanceGlyph.Overridden);
        public static readonly InheritanceTag Overrides = new InheritanceTag(InheritanceGlyph.Overrides);
        public static readonly InheritanceTag OverridesAndOverridden = new InheritanceTag(InheritanceGlyph.OverridesAndOverridden);
    }
}
