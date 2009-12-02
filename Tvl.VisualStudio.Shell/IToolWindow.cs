namespace Tvl.VisualStudio.Shell
{
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Shell;

    public interface IToolWindow
    {
        string Caption
        {
            get;
        }

        ImageSource Icon
        {
            get;
        }

        FrameworkElement VisualElement
        {
            get;
        }

        bool MultiInstance
        {
            get;
        }

        ToolWindowOrientation Orientation
        {
            get;
        }

        VsDockStyle Style
        {
            get;
        }

        bool Transient
        {
            get;
        }
    }
}
