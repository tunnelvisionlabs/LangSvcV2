namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.Collections.Generic;

    public interface IInheritanceTagFactory
    {
        IInheritanceTag CreateTag(InheritanceGlyph glyph, string displayName, IEnumerable<IInheritanceTarget> targets);
    }
}
