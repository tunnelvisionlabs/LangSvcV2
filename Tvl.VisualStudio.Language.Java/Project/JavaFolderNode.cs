namespace Tvl.VisualStudio.Language.Java.Project
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Project;

    using __VSHPROPID = Microsoft.VisualStudio.Shell.Interop.__VSHPROPID;

    [ComVisible(true)]
    public class JavaFolderNode : FolderNode
    {
        public JavaFolderNode(ProjectNode root, string relativePath, ProjectElement element)
            : base(root, relativePath, element)
        {
            SetProperty((int)__VSHPROPID.VSHPROPID_IsNonMemberItem, false);
        }

        public new JavaProjectNode ProjectManager
        {
            get
            {
                return (JavaProjectNode)base.ProjectManager;
            }
        }

        public override object GetIconHandle(bool open)
        {
            if (string.Equals(ItemNode.ItemName, JavaProjectFileConstants.SourceFolder))
                return this.ProjectManager.ExtendedImageHandler.GetIconHandle(open ? (int)JavaProjectNode.ExtendedImageName.OpenSourceFolder : (int)JavaProjectNode.ExtendedImageName.SourceFolder);

            if (string.Equals(ItemNode.ItemName, JavaProjectFileConstants.TestSourceFolder))
                return this.ProjectManager.ExtendedImageHandler.GetIconHandle(open ? (int)JavaProjectNode.ExtendedImageName.OpenTestSourceFolder: (int)JavaProjectNode.ExtendedImageName.TestSourceFolder);

            return base.GetIconHandle(open);
        }

        protected override NodeProperties CreatePropertiesObject()
        {
            return new JavaFolderNodeProperties(this);
        }
    }
}
