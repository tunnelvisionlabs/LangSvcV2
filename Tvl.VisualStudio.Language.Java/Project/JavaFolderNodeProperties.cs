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
        [DefaultValue("None")]
        public virtual string BuildAction
        {
            get
            {
                string value = this.Node.ItemNode.ItemName;
                if (string.IsNullOrEmpty(value))
                    return "None";

                return value;
            }

            set
            {
                this.Node.ItemNode.ItemName = value;
            }
        }
    }
}
