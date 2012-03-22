namespace Tvl.VisualStudio.MouseNavigation
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    using SVsServiceProvider = Microsoft.VisualStudio.Shell.SVsServiceProvider;
    using UIElement = System.Windows.UIElement;

    [Export(typeof(IMouseProcessorProvider))]
    [Order]
    [ContentType("text")]
    [Name("MouseNavigation")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    public class MouseNavigationProvider : IMouseProcessorProvider
    {
        [Import]
        public SVsServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            if (!(wpfTextView is UIElement))
                return null;

            return new MouseNavigationProcessor(wpfTextView, ServiceProvider);
        }
    }
}
