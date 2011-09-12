/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.VisualStudio.Project.Automation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    [ComVisible(true)]
    [CLSCompliant(false)]
    public class OANestedProjectItem : OAProjectItem<NestedProjectNode>
    {
        private readonly EnvDTE.Project nestedProject;

        public OANestedProjectItem(OAProject project, NestedProjectNode node)
            : base(project, node)
        {
            Contract.Requires<ArgumentNullException>(project != null, "project");
            Contract.Requires<ArgumentNullException>(node != null, "node");

            object nestedproject;
            if (ErrorHandler.Succeeded(node.NestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out nestedproject)))
            {
                this.nestedProject = nestedproject as EnvDTE.Project;
            }
        }

        /// <summary>
        /// Returns the collection of project items defined in the nested project
        /// </summary>
        public override EnvDTE.ProjectItems ProjectItems
        {
            get
            {
                if (this.nestedProject != null)
                {
                    return this.nestedProject.ProjectItems;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the nested project.
        /// </summary>
        public override EnvDTE.Project SubProject
        {
            get
            {
                return this.nestedProject;
            }
        }
    }
}
