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
    }
}
