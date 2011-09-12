namespace Tvl.VisualStudio.Language.Java.Project.Automation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Project.Automation;

    [ComVisible(true)]
    public class OAJarReference : OAReferenceBase<JarReferenceNode>
    {
        public OAJarReference(JarReferenceNode jarReference)
            : base(jarReference)
        {
        }

#warning implement this
    }
}
