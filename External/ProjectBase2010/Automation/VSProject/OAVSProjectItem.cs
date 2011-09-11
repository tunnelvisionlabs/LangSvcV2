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
    using System.Runtime.InteropServices;
    using EnvDTE;
    using VSLangProj;

    /// <summary>
    /// Represents a language-specific project item
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OAVS")]
    [CLSCompliant(false)]
    [ComVisible(true)]
    public class OAVSProjectItem : VSProjectItem
    {
        private FileNode fileNode;

        public OAVSProjectItem(FileNode fileNode)
        {
            this.FileNode = fileNode;
        }

        #region VSProjectItem Members

        public virtual EnvDTE.Project ContainingProject
        {
            get
            {
                return fileNode.ProjectManager.GetAutomationObject() as EnvDTE.Project;
            }
        }

        public virtual ProjectItem ProjectItem
        {
            get
            {
                return fileNode.GetAutomationObject() as ProjectItem;
            }
        }

        public virtual DTE DTE
        {
            get
            {
                return (DTE)this.fileNode.ProjectManager.Site.GetService(typeof(DTE));
            }
        }

        public virtual void RunCustomTool()
        {
            this.FileNode.RunGenerator();
        }

        #endregion

        #region public properties

        /// <summary>
        /// File Node property
        /// </summary>
        public FileNode FileNode
        {
            get
            {
                return fileNode;
            }

            set
            {
                fileNode = value;
            }
        }

        #endregion

    }
}
