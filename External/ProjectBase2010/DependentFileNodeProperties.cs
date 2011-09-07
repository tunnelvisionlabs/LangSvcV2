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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
	[CLSCompliant(false), ComVisible(true)]
	public class DependentFileNodeProperties : NodeProperties
	{
		#region properties
		[SRCategoryAttribute(SR.Advanced)]
		[LocDisplayName(SR.BuildAction)]
		[SRDescriptionAttribute(SR.BuildActionDescription)]
		public virtual BuildAction BuildAction
		{
			get
			{
				string value = this.Node.ItemNode.ItemName;
				if(value == null || value.Length == 0)
				{
					return BuildAction.None;
				}
				return (BuildAction)Enum.Parse(typeof(BuildAction), value);
			}
			set
			{
				this.Node.ItemNode.ItemName = value.ToString();
			}
		}

		[SRCategoryAttribute(SR.Misc)]
		[LocDisplayName(SR.FileName)]
		[SRDescriptionAttribute(SR.FileNameDescription)]
		public virtual string FileName
		{
			get
			{
				return this.Node.Caption;
			}
		}

		[SRCategoryAttribute(SR.Misc)]
		[LocDisplayName(SR.FullPath)]
		[SRDescriptionAttribute(SR.FullPathDescription)]
		public string FullPath
		{
			get
			{
				return this.Node.Url;
			}
		}
		#endregion

		#region ctors
		public DependentFileNodeProperties(HierarchyNode node)
			: base(node)
		{
		}

		#endregion

		#region overridden methods
		public override string GetClassName()
		{
			return SR.GetString(SR.FileProperties, CultureInfo.CurrentUICulture);
		}
		#endregion
	}
}
