namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Name("InheritanceGlyphFactory")]
    [Export(typeof(IGlyphFactoryProvider))]
    [ContentType("text")]
    [TagType(typeof(InheritanceTag))]
    [Order]
    public class InheritanceGlyphFactoryProvider : IGlyphFactoryProvider
    {
        #region IGlyphFactoryProvider Members

        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            if (view == null || margin == null)
                return null;

            return new InheritanceGlyphFactory(this, view, margin);
        }

        #endregion
    }
}
