namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System.Windows;
    using System.Windows.Controls;

    public partial class AstExplorerTreeControl : UserControl
    {
        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged
        {
            add
            {
                treeView1.SelectedItemChanged += value;
            }
            remove
            {
                treeView1.SelectedItemChanged -= value;
            }
        }

        public AstExplorerTreeControl()
        {
            InitializeComponent();
        }

        public ItemCollection Items
        {
            get
            {
                return treeView1.Items;
            }
        }
    }
}
