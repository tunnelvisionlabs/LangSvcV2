namespace JavaLanguageService.Panes
{
    using System.Windows;
    using System.Windows.Media;

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
    }
}
