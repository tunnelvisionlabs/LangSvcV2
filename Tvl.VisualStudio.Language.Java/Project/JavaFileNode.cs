namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Project;
    using __VSHPROPID = Microsoft.VisualStudio.Shell.Interop.__VSHPROPID;

    public class JavaFileNode : FileNode
    {
        public JavaFileNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            SetProperty((int)__VSHPROPID.VSHPROPID_IsNonMemberItem, false);
        }
    }
}
