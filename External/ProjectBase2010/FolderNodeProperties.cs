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

namespace Microsoft.VisualStudio.Project
{
	[CLSCompliant(false), ComVisible(true)]
	public class FolderNodeProperties : NodeProperties
	{
		#region properties
		[SRCategoryAttribute(SR.Misc)]
		[LocDisplayName(SR.FolderName)]
		[SRDescriptionAttribute(SR.FolderNameDescription)]
		[AutomationBrowsable(false)]
		public string FolderName
		{
			get
			{
				return this.Node.Caption;
			}
			set
			{
				this.Node.SetEditLabel(value);
				this.Node.ReDraw(UIHierarchyElement.Caption);
			}
		}

		#region properties - used for automation only
		[Browsable(false)]
		[AutomationBrowsable(true)]
		public string FileName
		{
			get
			{
				return this.Node.Caption;
			}
			set
			{
				this.Node.SetEditLabel(value);
			}
		}

		[Browsable(false)]
		[AutomationBrowsable(true)]
		public string FullPath
		{
			get
			{
				string fullPath = this.Node.GetMkDocument();
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
		public FolderNodeProperties(HierarchyNode node)
			: base(node)
		{
		}
		#endregion

		#region overridden methods
		public override string GetClassName()
		{
			return SR.GetString(SR.FolderProperties, CultureInfo.CurrentUICulture);
		}
		#endregion
	}
}
