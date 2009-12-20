namespace Tvl.VisualStudio.Tools.FileList
{
    using System.Windows;
    using System.Windows.Media;
    using Tvl.VisualStudio.Shell;

    internal class SolutionFileListWindow : IToolWindow
    {
        private SolutionFileListProvider _provider;

        public SolutionFileListWindow(SolutionFileListProvider provider)
        {
            this._provider = provider;
        }

        public string Caption
        {
            get
            {
                return "Solution File List";
            }
        }

        public ImageSource Icon
        {
            get
            {
                return null;
            }
        }

        public FrameworkElement CreateContent()
        {
            return new SolutionFileListControl(this._provider);
        }
    }
}
