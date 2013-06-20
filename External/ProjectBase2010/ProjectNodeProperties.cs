/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
	[CLSCompliant(false), ComVisible(true)]
	public class ProjectNodeProperties : NodeProperties
	{
		#region properties
		[SRCategoryAttribute(SR.Misc)]
		[LocDisplayName(SR.ProjectFolder)]
		[SRDescriptionAttribute(SR.ProjectFolderDescription)]
		[AutomationBrowsable(false)]
		public string ProjectFolder
		{
			get
			{
				return this.Node.ProjectManager.ProjectFolder;
			}
		}

		[SRCategoryAttribute(SR.Misc)]
		[LocDisplayName(SR.ProjectFile)]
		[SRDescriptionAttribute(SR.ProjectFileDescription)]
		[AutomationBrowsable(false)]
		public string ProjectFile
		{
			get
			{
				return this.Node.ProjectManager.ProjectFile;
			}
			set
			{
				this.Node.ProjectManager.ProjectFile = value;
			}
		}

		#region non-browsable properties - used for automation only
		[Browsable(false)]
		public string FileName
		{
			get
			{
				return this.Node.ProjectManager.ProjectFile;
			}
			set
			{
				this.Node.ProjectManager.ProjectFile = value;
			}
		}


		[Browsable(false)]
		public string FullPath
		{
			get
			{
				string fullPath = this.Node.ProjectManager.ProjectFolder;
				if(!fullPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				{
					return fullPath + Path.DirectorySeparatorChar;
				}
				else
				{
					return fullPath;
				}
			}
		}
		#endregion

		#endregion

		#region ctors
		public ProjectNodeProperties(ProjectNode node)
			: base(node)
		{
		}
		#endregion

		#region overridden methods
		public override string GetClassName()
		{
			return SR.GetString(SR.ProjectProperties, CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// ICustomTypeDescriptor.GetEditor
		/// To enable the "Property Pages" button on the properties browser
		/// the browse object (project properties) need to be unmanaged
		/// or it needs to provide an editor of type ComponentEditor.
		/// </summary>
		/// <param name="editorBaseType">Type of the editor</param>
		/// <returns>Editor</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", 
            Justification="The service provider is used by the PropertiesEditorLauncher")]
        public override object GetEditor(Type editorBaseType)
		{
			// Override the scenario where we are asked for a ComponentEditor
			// as this is how the Properties Browser calls us
			if(editorBaseType == typeof(ComponentEditor))
			{
				IOleServiceProvider sp;
				ErrorHandler.ThrowOnFailure(this.Node.GetSite(out sp));
				return new PropertiesEditorLauncher(new ServiceProvider(sp));
			}

			return base.GetEditor(editorBaseType);
		}

		public override int GetCfgProvider(out IVsCfgProvider p)
		{
			if(this.Node != null && this.Node.ProjectManager != null)
			{
				return this.Node.ProjectManager.GetCfgProvider(out p);
			}

			return base.GetCfgProvider(out p);
		}
		#endregion
	}
}
