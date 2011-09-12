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
        private readonly FileNode _fileNode;

        public OAVSProjectItem(FileNode fileNode)
        {
            Contract.Requires<ArgumentNullException>(fileNode != null, "fileNode");
            this._fileNode = fileNode;
        }

        /// <summary>
        /// File Node property
        /// </summary>
        public FileNode FileNode
        {
            get
            {
                return _fileNode;
            }
        }

        #region VSProjectItem Members

        public virtual EnvDTE.Project ContainingProject
        {
            get
            {
                return _fileNode.ProjectManager.GetAutomationObject() as EnvDTE.Project;
            }
        }

        public virtual ProjectItem ProjectItem
        {
            get
            {
                return _fileNode.GetAutomationObject() as ProjectItem;
            }
        }

        public virtual DTE DTE
        {
            get
            {
                return (DTE)this._fileNode.ProjectManager.Site.GetService(typeof(DTE));
            }
        }

        public virtual void RunCustomTool()
        {
            this.FileNode.RunGenerator();
        }

        #endregion
    }
}
