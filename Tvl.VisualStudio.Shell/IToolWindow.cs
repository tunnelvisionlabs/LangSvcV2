namespace Tvl.VisualStudio.Shell
{
    using System.Windows;
    using System.Windows.Media.Imaging;

    public interface IToolWindow
    {
        string Caption
        {
            get;
        }

        BitmapSource Icon
        {
            get;
        }

        FrameworkElement CreateContent();

        /*
         * This property is replaced by an IMultiInstanceToolWindowProvider
         */
        //bool MultiInstance
        //{
        //    get;
        //}

        /*
         * This property is replaced by a DefaultPosition() attribute
         */
        //ToolWindowOrientation Orientation
        //{
        //    get;
        //}

        /*
         * This property is replaced by a DefaultPosition() attribute
         */
        //VsDockStyle Style
        //{
        //    get;
        //}

        //bool Transient
        //{
        //    get;
        //}
    }
}
