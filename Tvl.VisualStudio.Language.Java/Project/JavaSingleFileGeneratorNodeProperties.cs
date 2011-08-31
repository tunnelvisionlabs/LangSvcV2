namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Project;
    using System.Runtime.InteropServices;
    using System.ComponentModel;

    [ComVisible(true)]
    public class JavaSingleFileGeneratorNodeProperties : SingleFileGeneratorNodeProperties
    {
        public JavaSingleFileGeneratorNodeProperties(HierarchyNode node)
            : base(node)
        {
        }

        [Browsable(false)]
        public override BuildAction BuildAction
        {
            get
            {
                return base.BuildAction;
            }

            set
            {
                base.BuildAction = value;
            }
        }
    }
}
