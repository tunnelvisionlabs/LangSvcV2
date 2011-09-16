/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using System.Diagnostics.Contracts;

namespace Microsoft.VisualStudio.Project
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	public class FolderNode : HierarchyNode, IProjectSourceNode
	{
        private bool isNonMemberItem;

		#region ctors
		/// <summary>
		/// Constructor for the FolderNode
		/// </summary>
		/// <param name="root">Root node of the hierarchy</param>
		/// <param name="relativePath">relative path from root i.e.: "NewFolder1\\NewFolder2\\NewFolder3</param>
		/// <param name="element">Associated project element</param>
		public FolderNode(ProjectNode root, string relativePath, ProjectElement element)
			: base(root, element)
		{
            Contract.Requires(root != null);
            Contract.Requires(element != null);
            Contract.Requires<ArgumentNullException>(relativePath != null, "relativePath");

			this.VirtualNodeName = relativePath.TrimEnd('\\');
            this.isNonMemberItem = element.IsVirtual;
            ExcludeNodeFromScc = true;
		}
		#endregion

		#region overridden properties
        /// <summary>
        /// Specifies if a Node is under source control.
        /// </summary>
        /// <value>Specifies if a Node is under source control.</value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Scc")]
        public override bool ExcludeNodeFromScc
        {
            get
            {
                // Non member items donot participate in SCC.
                if (this.IsNonMemberItem)
                {
                    return true;
                }

                return base.ExcludeNodeFromScc;
            }

            set
            {
                base.ExcludeNodeFromScc = value;
            }
        }

		public override int SortPriority
		{
			get { return DefaultSortOrderNode.FolderNode; }
		}

		/// <summary>
		/// This relates to the SCC glyph
		/// </summary>
		public override VsStateIcon StateIconIndex
		{
			get
			{
				// The SCC manager does not support being asked for the state icon of a folder (result of the operation is undefined)
				return VsStateIcon.STATEICON_NOSTATEICON;
			}
		}
		#endregion

        // =========================================================================================
        // IProjectSourceNode Properties
        // =========================================================================================

        /// <summary>
        /// Flag that indicates if this node is not a member of the project.
        /// </summary>
        /// <value>true if the item is not a member of the project build, false otherwise.</value>
        public bool IsNonMemberItem
        {
            get
            {
                return this.isNonMemberItem;
            }

            set
            {
                this.isNonMemberItem = value;
            }
        }

		#region overridden methods
		protected override NodeProperties CreatePropertiesObject()
		{
			return new FolderNodeProperties(this);
		}

		protected internal override void DeleteFromStorage(string path)
		{
			this.DeleteFolder(path);
		}

		/// <summary>
		/// Get the automation object for the FolderNode
		/// </summary>
		/// <returns>An instance of the Automation.OAFolderNode type if succeeded</returns>
		public override object GetAutomationObject()
		{
			if(this.ProjectManager == null || this.ProjectManager.IsClosed)
			{
				return null;
			}

			return new Automation.OAFolderItem(this.ProjectManager.GetAutomationObject() as Automation.OAProject, this);
		}

        /// <summary>
        /// Sets the node property.
        /// </summary>
        /// <param name="propid">Property id.</param>
        /// <param name="value">Property value.</param>
        /// <returns>Returns success or failure code.</returns>
        public override int SetProperty(int propid, object value)
        {
            int result;
            __VSHPROPID id = (__VSHPROPID)propid;
            switch (id)
            {
            case __VSHPROPID.VSHPROPID_IsNonMemberItem:
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                bool boolValue = false;
                CCITracing.TraceCall(this.ID + "," + id.ToString());
                if (bool.TryParse(value.ToString(), out boolValue))
                {
                    this.IsNonMemberItem = boolValue;
                }
                else
                {
                    Trace.WriteLine("Could not parse the IsNonMemberItem property value.");
                }

                result = VSConstants.S_OK;
                break;

            default:
                result = base.SetProperty(propid, value);
                break;
            }

            return result;
        }

        /// <summary>
        /// Gets the node property.
        /// </summary>
        /// <param name="propId">Property id.</param>
        /// <returns>The property value.</returns>
        public override object GetProperty(int propId)
        {
            switch ((__VSHPROPID)propId)
            {
            case __VSHPROPID.VSHPROPID_IsNonMemberItem:
                return this.IsNonMemberItem;
            }

            return base.GetProperty(propId);
        }

        /// <summary>
        /// Provides the node name for inline editing of caption. 
        /// Overriden to diable this fuctionality for non member fodler node.
        /// </summary>
        /// <returns>Caption of the folder node if the node is a member item, null otherwise.</returns>
        public override string GetEditLabel()
        {
            if (this.IsNonMemberItem)
            {
                return null;
            }

            return base.GetEditLabel();
        }

		public override object GetIconHandle(bool open)
		{
            if (this.IsNonMemberItem)
            {
                return this.ProjectManager.ImageHandler.GetIconHandle(open ? (int)ProjectNode.ImageName.OpenExcludedFolder : (int)ProjectNode.ImageName.ExcludedFolder);
            }

			return this.ProjectManager.ImageHandler.GetIconHandle(open ? (int)ProjectNode.ImageName.OpenFolder : (int)ProjectNode.ImageName.Folder);
		}

        /// <summary>
        /// Collapses the folder.
        /// </summary>
        public void CollapseFolder()
        {
            this.SetExpanded(false);
        }

        /// <summary>
        /// Expands the folder.
        /// </summary>
        public void ExpandFolder()
        {
            this.SetExpanded(true);
        }

		/// <summary>
		/// Rename Folder
		/// </summary>
		/// <param name="label">new Name of Folder</param>
		/// <returns>VSConstants.S_OK, if succeeded</returns>
		public override int SetEditLabel(string label)
		{
			if(String.Equals(Path.GetFileName(this.Url.TrimEnd('\\')), label, StringComparison.Ordinal))
			{
				// Label matches current Name
				return VSConstants.S_OK;
			}

			string newPath = Path.Combine(new DirectoryInfo(this.Url).Parent.FullName, label);

			// Verify that No Directory/file already exists with the new name among current children
			for(HierarchyNode n = Parent.FirstChild; n != null; n = n.NextSibling)
			{
				if(n != this && String.Equals(n.Caption, label, StringComparison.OrdinalIgnoreCase))
				{
					return ShowFileOrFolderAlreadExistsErrorMessage(newPath);
				}
			}

			// Verify that No Directory/file already exists with the new name on disk
			if(Directory.Exists(newPath) || File.Exists(newPath))
			{
				return ShowFileOrFolderAlreadExistsErrorMessage(newPath);
			}

			try
			{
				RenameFolder(label);

				//Refresh the properties in the properties window
				IVsUIShell shell = this.ProjectManager.GetService(typeof(SVsUIShell)) as IVsUIShell;
				Debug.Assert(shell != null, "Could not get the ui shell from the project");
				ErrorHandler.ThrowOnFailure(shell.RefreshPropertyBrowser(0));

				// Notify the listeners that the name of this folder is changed. This will
				// also force a refresh of the SolutionExplorer's node.
				this.OnPropertyChanged(this, (int)__VSHPROPID.VSHPROPID_Caption, 0);
			}
			catch(Exception e)
			{
				throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, SR.GetString(SR.RenameFolder, CultureInfo.CurrentUICulture), e.Message));
			}
			return VSConstants.S_OK;
		}


        public override int MenuCommandId
        {
            get
            {
                if (this.IsNonMemberItem)
                {
                    return VsMenus.IDM_VS_CTXT_XPROJ_MULTIITEM;
                }

                return VsMenus.IDM_VS_CTXT_FOLDERNODE;
            }
        }

		public override Guid ItemTypeGuid
		{
			get
			{
				return VSConstants.GUID_ItemType_PhysicalFolder;
			}
		}

		public override string Url
		{
			get
			{
				return Path.Combine(Path.GetDirectoryName(this.ProjectManager.Url), this.VirtualNodeName) + "\\";
			}
		}

		public override string Caption
		{
			get
			{
				// it might have a backslash at the end... 
				// and it might consist of Grandparent\parent\this\
				string caption = this.VirtualNodeName;
				string[] parts;
				parts = caption.Split(Path.DirectorySeparatorChar);
				caption = parts[parts.GetUpperBound(0)];
				return caption;
			}
		}

		public override string GetMkDocument()
		{
			Debug.Assert(this.Url != null, "No url sepcified for this node");

			return this.Url;
		}

		/// <summary>
		/// Enumerate the files associated with this node.
		/// A folder node is not a file and as such no file to enumerate.
		/// </summary>
		/// <param name="files">The list of files to be placed under source control.</param>
		/// <param name="flags">The flags that are associated to the files.</param>
		protected internal override void GetSccFiles(System.Collections.Generic.IList<string> files, System.Collections.Generic.IList<tagVsSccFilesFlags> flags)
		{
			return;
		}

		/// <summary>
		/// This method should be overridden to provide the list of special files and associated flags for source control.
		/// </summary>
		/// <param name="sccFile">One of the file associated to the node.</param>
		/// <param name="files">The list of files to be placed under source control.</param>
		/// <param name="flags">The flags that are associated to the files.</param>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Scc")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "scc")]
		protected internal override void GetSccSpecialFiles(string sccFile, IList<string> files, IList<tagVsSccFilesFlags> flags)
		{
			if(this.ExcludeNodeFromScc)
			{
				return;
			}

			if(files == null)
			{
				throw new ArgumentNullException("files");
			}

			if(flags == null)
			{
				throw new ArgumentNullException("flags");
			}

			if(string.IsNullOrEmpty(sccFile))
			{
				throw new ArgumentException(SR.GetString(SR.InvalidParameter, CultureInfo.CurrentUICulture), "sccFile");
			}

			// Get the file node for the file passed in.
			FileNode node = this.FindChild(sccFile) as FileNode;

			// Dependents do not participate directly in scc.
			if(node != null && !(node is DependentFileNode))
			{
				node.GetSccSpecialFiles(sccFile, files, flags);
			}
		}

		/// <summary>
		/// Recursevily walks the folder nodes and redraws the state icons
		/// </summary>
		protected internal override void UpdateSccStateIcons()
		{
			for(HierarchyNode child = this.FirstChild; child != null; child = child.NextSibling)
			{
				child.UpdateSccStateIcons();
			}
		}

		protected override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
		{
			if(cmdGroup == VsMenus.guidStandardCommandSet97)
			{
				switch((VsCommands)cmd)
				{
					case VsCommands.Copy:
					case VsCommands.Paste:
					case VsCommands.Cut:
					case VsCommands.Rename:
						result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
						return VSConstants.S_OK;

					case VsCommands.NewFolder:
					case VsCommands.AddNewItem:
					case VsCommands.AddExistingItem:
						result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
						return VSConstants.S_OK;
				}
			}
			else if(cmdGroup == VsMenus.guidStandardCommandSet2K)
			{
                if ((VsCommands2K)cmd == VsCommands2K.INCLUDEINPROJECT)
                {
                    // if it is a non member item node, the we support "Include In Project" command
                    if (IsNonMemberItem)
                    {
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                    }
                    else
                    {
                        result |= QueryStatusResult.NOTSUPPORTED;
                    }

                    return VSConstants.S_OK;
                }
                else if ((VsCommands2K)cmd == VsCommands2K.EXCLUDEFROMPROJECT)
				{
                    // if it is a non member item node, then we don't support "Exclude From Project" command
                    if (IsNonMemberItem)
                    {
                        result |= QueryStatusResult.NOTSUPPORTED;
                    }
                    else
                    {
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                    }

                    return VSConstants.S_OK;
				}
			}
			else
			{
				return (int)OleConstants.OLECMDERR_E_UNKNOWNGROUP;
			}
			return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
		}

        protected override int ExecCommandOnNode(Guid cmdGroup, uint cmd, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (cmdGroup == VsMenus.guidStandardCommandSet2K)
            {
                switch ((VsCommands2K)cmd)
                {
                case VsCommands2K.INCLUDEINPROJECT:
                    return ((IProjectSourceNode)this).IncludeInProject();

                case VsCommands2K.EXCLUDEFROMPROJECT:
                    return ((IProjectSourceNode)this).ExcludeFromProject();

                case ProjectFileConstants.CommandExploreFolderInWindows:
                    ProjectNode.ExploreFolderInWindows(GetMkDocument());
                    return VSConstants.S_OK;
                }
            }

            return base.ExecCommandOnNode(cmdGroup, cmd, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// Adds the this node to the build system.
        /// </summary>
        /// <param name="recursive">Flag to indicate if the addition should be recursive.</param>
        protected virtual void AddToMSBuild(bool recursive)
        {
            if (ProjectManager == null || ProjectManager.IsClosed)
            {
                return; // do nothing
            }

            this.ItemNode = ProjectManager.AddFileToMsBuild(this.Url);
            this.SetProperty((int)__VSHPROPID.VSHPROPID_IsNonMemberItem, false);
            if (recursive)
            {
                for (HierarchyNode node = this.FirstChild; node != null; node = node.NextSibling)
                {
                    IProjectSourceNode sourceNode = node as IProjectSourceNode;
                    if (sourceNode != null)
                    {
                        sourceNode.IncludeInProject(recursive);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the expanded state of the folder.
        /// </summary>
        /// <param name="expanded">Flag that indicates the expanded state of the folder.
        /// This should be 'true' for expanded and 'false' for collapsed state.</param>
        protected void SetExpanded(bool expanded)
        {
            this.IsExpanded = expanded;
            this.SetProperty((int)__VSHPROPID.VSHPROPID_Expanded, expanded);

            // If we are in automation mode then skip the ui part
            if (!Utilities.IsInAutomationFunction(this.ProjectManager.Site))
            {
                IVsUIHierarchyWindow uiWindow = UIHierarchyUtilities.GetUIHierarchyWindow(this.ProjectManager.Site, SolutionExplorer);
                int result = uiWindow.ExpandItem(this.ProjectManager, this.ID, expanded ? EXPANDFLAGS.EXPF_ExpandFolder : EXPANDFLAGS.EXPF_CollapseFolder);
                ErrorHandler.ThrowOnFailure(result);

                // then post the expand command to the shell. Folder verification and creation will
                // happen in the setlabel code...
                IVsUIShell shell = ProjectManager.Site.GetService(typeof(SVsUIShell)) as IVsUIShell;

                object dummy = null;
                Guid cmdGroup = VsMenus.guidStandardCommandSet97;
                result = shell.PostExecCommand(ref cmdGroup, (uint)(expanded ? VsCommands.Expand : VsCommands.Collapse), 0, ref dummy);
                ErrorHandler.ThrowOnFailure(result);
            }
        }

		protected override bool CanDeleteItem(__VSDELETEITEMOPERATION deleteOperation)
		{
			if(deleteOperation == __VSDELETEITEMOPERATION.DELITEMOP_DeleteFromStorage)
			{
				return this.ProjectManager.CanProjectDeleteItems;
			}
			return false;
		}

        // =========================================================================================
        // IProjectSourceNode Methods
        // =========================================================================================

        /// <summary>
        /// Exclude the item from the project system.
        /// </summary>
        /// <returns>Returns success or failure code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        int IProjectSourceNode.ExcludeFromProject()
        {
            if (ProjectManager == null || ProjectManager.IsClosed)
            {
                return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
            }
            else if (this.IsNonMemberItem)
            {
                return VSConstants.S_OK; // do nothing, just ignore it.
            }

            //using ( WixHelperMethods.NewWaitCursor() )
            //{
            // Check out the project file.
            if (!ProjectManager.QueryEditProjectFile(false))
            {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            // remove children, if any, before removing from the hierarchy
            for (HierarchyNode child = this.FirstChild; child != null; child = child.NextSibling)
            {
                IProjectSourceNode node = child as IProjectSourceNode;
                if (node != null)
                {
                    int result = node.ExcludeFromProject();
                    if (result != VSConstants.S_OK)
                    {
                        return result;
                    }
                }
            }

            if (ProjectManager != null && ProjectManager.ShowAllFilesEnabled && Directory.Exists(this.Url))
            {
                string url = this.Url;
                this.SetProperty((int)__VSHPROPID.VSHPROPID_IsNonMemberItem, true);
                this.ItemNode.RemoveFromProjectFile();
                this.ItemNode = new ProjectElement(this.ProjectManager, null, true);  // now we have to create a new ItemNode to indicate that this is virtual node.
                this.ItemNode.Rename(url);
                this.ItemNode.SetMetadata(ProjectFileConstants.Name, this.Url);
                this.ReDraw(UIHierarchyElement.Icon); // we have to redraw the icon of the node as it is now not a member of the project and shoul be drawn using a different icon.
            }
            else if (this.Parent != null) // the project node has no parentNode
            {
                // this is important to make it non member item. otherwise, the multi-selection scenario would
                // not work if it has any parent child relation.
                this.SetProperty((int)__VSHPROPID.VSHPROPID_IsNonMemberItem, true);

                // remove from the hierarchy
                this.OnItemDeleted();
                this.Parent.RemoveChild(this);
                this.ItemNode.RemoveFromProjectFile();
            }

            // refresh property browser...
            ProjectNode.RefreshPropertyBrowser();
            //}

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Include the item into the project system.
        /// </summary>
        /// <returns>Returns success or failure code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        int IProjectSourceNode.IncludeInProject()
        {
            return ((IProjectSourceNode)this).IncludeInProject(true);
        }

        /// <summary>
        /// Include the item into the project system recursively.
        /// </summary>
        /// <param name="recursive">Flag that indicates if the inclusion should be recursive or not.</param>
        /// <returns>Returns success or failure code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        int IProjectSourceNode.IncludeInProject(bool recursive)
        {
            if (this.ProjectManager == null || this.ProjectManager.IsClosed)
            {
                return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
            }
            else if (!this.IsNonMemberItem)
            {
                return VSConstants.S_OK; // do nothing, just ignore it.
            }

            //using ( WixHelperMethods.NewWaitCursor() )
            //{
            // Check out the project file.
            if (!this.ProjectManager.QueryEditProjectFile(false))
            {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            // make sure that all parent folders are included in the project
            ProjectNode.EnsureParentFolderIncluded(this);

            // now add this node to the project.
            this.AddToMSBuild(recursive);
            this.ReDraw(UIHierarchyElement.Icon);

            // refresh property browser...
            ProjectNode.RefreshPropertyBrowser();
            //}

            return VSConstants.S_OK;
        }
        #endregion

		#region virtual methods
		/// <summary>
		/// Override if your node is not a file system folder so that
		/// it does nothing or it deletes it from your storage location.
		/// </summary>
		/// <param name="path">Path to the folder to delete</param>
		public virtual void DeleteFolder(string path)
		{
			if(Directory.Exists(path))
				Directory.Delete(path, true);
		}

		/// <summary>
		/// creates the physical directory for a folder node
		/// Override if your node does not use file system folder
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "e")]
		public virtual void CreateDirectory()
		{
			try
			{
				if(Directory.Exists(this.Url) == false)
				{
					Directory.CreateDirectory(this.Url);
				}
			}
			//TODO - this should not digest all exceptions.
			catch(System.Exception e)
			{
				CCITracing.Trace(e);
				throw;
			}
		}
		/// <summary>
		/// Creates a folder nodes physical directory
		/// Override if your node does not use file system folder
		/// </summary>
		/// <param name="newName"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "e")]
		public virtual void CreateDirectory(string newName)
		{
			if(String.IsNullOrEmpty(newName))
			{
				throw new ArgumentException(SR.GetString(SR.ParameterCannotBeNullOrEmpty, CultureInfo.CurrentUICulture), "newName");
			}

			try
			{
				// on a new dir && enter, we get called with the same name (so do nothing if name is the same
				char[] dummy = new char[1];
				dummy[0] = Path.DirectorySeparatorChar;
				string oldDir = this.Url;
				oldDir = oldDir.TrimEnd(dummy);
				string strNewDir = Path.Combine(Path.GetDirectoryName(oldDir), newName);

				if(!String.Equals(strNewDir, oldDir, StringComparison.OrdinalIgnoreCase))
				{
					if(Directory.Exists(strNewDir))
					{
						throw new InvalidOperationException(SR.GetString(SR.DirectoryExistError, CultureInfo.CurrentUICulture));
					}
					Directory.CreateDirectory(strNewDir);
				}
			}
			//TODO - this should not digest all exceptions.
			catch(System.Exception e)
			{
				CCITracing.Trace(e);
				throw;
			}
		}

		/// <summary>
		/// Rename the physical directory for a folder node
		/// Override if your node does not use file system folder
		/// </summary>
		/// <returns></returns>
		public virtual void RenameDirectory(string newPath)
		{
			if(Directory.Exists(this.Url))
			{
				if(Directory.Exists(newPath))
				{
					ShowFileOrFolderAlreadExistsErrorMessage(newPath);
				}

				Directory.Move(this.Url, newPath);
			}
		}
		#endregion

		#region helper methods
		private void RenameFolder(string newName)
		{
			// Do the rename (note that we only do the physical rename if the leaf name changed)
			string newPath = Path.Combine(this.Parent.VirtualNodeName, newName);
			if(!String.Equals(Path.GetFileName(VirtualNodeName), newName, StringComparison.Ordinal))
			{
				this.RenameDirectory(Path.Combine(this.ProjectManager.ProjectFolder, newPath));
			}
			this.VirtualNodeName = newPath;

			this.ItemNode.Rename(VirtualNodeName);

			// Let all children know of the new path
			for(HierarchyNode child = this.FirstChild; child != null; child = child.NextSibling)
			{
				FolderNode node = child as FolderNode;

				if(node == null)
				{
					child.SetEditLabel(child.Caption);
				}
				else
				{
					node.RenameFolder(node.Caption);
				}
			}

			// Some of the previous operation may have changed the selection so set it back to us
			IVsUIHierarchyWindow uiWindow = UIHierarchyUtilities.GetUIHierarchyWindow(this.ProjectManager.Site, SolutionExplorer);
			// This happens in the context of renaming a folder.
			// Since we are already in solution explorer, it is extremely unlikely that we get a null return.
			// If we do, the consequences are minimal: the parent node will be selected instead of the
			// renamed node.
			if (uiWindow != null)
			{
				ErrorHandler.ThrowOnFailure(uiWindow.ExpandItem(this.ProjectManager, this.ID, EXPANDFLAGS.EXPF_SelectItem));
			}
		}

		/// <summary>
		/// Show error message if not in automation mode, otherwise throw exception
		/// </summary>
		/// <param name="newPath">path of file or folder already existing on disk</param>
		/// <returns>S_OK</returns>
		private int ShowFileOrFolderAlreadExistsErrorMessage(string newPath)
		{
			//A file or folder with the name '{0}' already exists on disk at this location. Please choose another name.
			//If this file or folder does not appear in the Solution Explorer, then it is not currently part of your project. To view files which exist on disk, but are not in the project, select Show All Files from the Project menu.
			string errorMessage = (String.Format(CultureInfo.CurrentCulture, SR.GetString(SR.FileOrFolderAlreadyExists, CultureInfo.CurrentUICulture), newPath));
			if(!Utilities.IsInAutomationFunction(this.ProjectManager.Site))
			{
				string title = null;
				OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
				OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
				OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
				VsShellUtilities.ShowMessageBox(this.ProjectManager.Site, title, errorMessage, icon, buttons, defaultButton);
				return VSConstants.S_OK;
			}
			else
			{
				throw new InvalidOperationException(errorMessage);
			}
		}

		#endregion
	}
}
