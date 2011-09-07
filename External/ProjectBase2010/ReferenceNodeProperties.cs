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
	public class ReferenceNodeProperties : NodeProperties
	{
		#region properties
		[SRCategoryAttribute(SR.Misc)]
		[LocDisplayName(SR.RefName)]
		[SRDescriptionAttribute(SR.RefNameDescription)]
		[Browsable(true)]
		[AutomationBrowsable(true)]
		public override string Name
		{
			get
			{
				return this.Node.Caption;
			}
		}

		[SRCategoryAttribute(SR.Misc)]
		[LocDisplayName(SR.CopyToLocal)]
		[SRDescriptionAttribute(SR.CopyToLocalDescription)]
		public bool CopyToLocal
		{
			get
			{
				string copyLocal = this.GetProperty(ProjectFileConstants.Private, "False");
				if(copyLocal == null || copyLocal.Length == 0)
					return true;
				return bool.Parse(copyLocal);
			}
			set
			{
				this.SetProperty(ProjectFileConstants.Private, value.ToString());
			}
		}

		[SRCategoryAttribute(SR.Misc)]
		[LocDisplayName(SR.FullPath)]
		[SRDescriptionAttribute(SR.FullPathDescription)]
		public virtual string FullPath
		{
			get
			{
				return this.Node.Url;
			}
		}
		#endregion

		#region ctors
		public ReferenceNodeProperties(HierarchyNode node)
			: base(node)
		{
		}
		#endregion

		#region overridden methods
		public override string GetClassName()
		{
			return SR.GetString(SR.ReferenceProperties, CultureInfo.CurrentUICulture);
		}
		#endregion
	}
}
