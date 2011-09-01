namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Project;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class JavaFolderNodeProperties : FolderNodeProperties
    {
        public JavaFolderNodeProperties(HierarchyNode node)
            : base(node)
        {
        }

        [Category("Advanced")]
        [DisplayName("Build Action")]
        [DefaultValue(FolderBuildAction.Folder)]
        public virtual FolderBuildAction BuildAction
        {
            get
            {
                string value = this.Node.ItemNode.ItemName;
                if (string.IsNullOrEmpty(value))
                    return FolderBuildAction.Folder;

                FolderBuildAction result;
                if (!Enum.TryParse(value, out result))
                    result = FolderBuildAction.Folder;

                return result;
            }

            set
            {
                this.Node.ItemNode.ItemName = value.ToString();
            }
        }
    }
}
