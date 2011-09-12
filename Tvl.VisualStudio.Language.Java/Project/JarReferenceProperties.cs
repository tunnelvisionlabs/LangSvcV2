namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Project;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class JarReferenceProperties : ReferenceNodeProperties
    {
        public JarReferenceProperties(JarReferenceNode node)
            : base(node)
        {
        }
    }
}
