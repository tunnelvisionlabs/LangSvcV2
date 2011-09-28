namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Tagging;

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
