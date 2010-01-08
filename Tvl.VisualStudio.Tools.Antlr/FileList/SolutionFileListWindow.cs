namespace Tvl.VisualStudio.Tools.FileList
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Tvl.VisualStudio.Shell;

    internal class SolutionFileListWindow : IToolWindow
    {
        private SolutionFileListProvider _provider;
        private BitmapSource _icon;

        public SolutionFileListWindow(SolutionFileListProvider provider)
        {
            this._provider = provider;

            string assemblyName = typeof(SolutionFileListWindow).Assembly.GetName().Name;
            this._icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/{0};component/Resources/filelist.png", assemblyName)));
        }

        public string Caption
        {
            get
            {
                return "Solution File List";
            }
        }

        public BitmapSource Icon
        {
            get
            {
                return _icon;
            }
        }

        public FrameworkElement CreateContent()
        {
            return new SolutionFileListControl(this._provider);
        }
    }
}
