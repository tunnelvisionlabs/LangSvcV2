namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Shell;
    using SVsServiceProvider = Microsoft.VisualStudio.Shell.SVsServiceProvider;

    [Name("InheritanceGlyphFactory")]
    [Export(typeof(IGlyphFactoryProvider))]
    [ContentType("text")]
    [TagType(typeof(InheritanceTag))]
    [Order]
    public class InheritanceGlyphFactoryProvider : IGlyphFactoryProvider
    {
        private static bool _packageLoaded;

        [Import]
        private SVsServiceProvider ServiceProvider
        {
            get;
            set;
        }

        #region IGlyphFactoryProvider Members

        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            if (view == null || margin == null)
                return null;

            if (!_packageLoaded)
            {
                ServiceProvider.GetShell().LoadPackage<InheritanceMarginPackage>();
                _packageLoaded = true;
            }

            return new InheritanceGlyphFactory(this, view, margin);
        }

        #endregion
    }
}
