/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using prjBuildAction = VSLangProj.prjBuildAction;

	[CLSCompliant(false), ComVisible(true)]
	public class DependentFileNodeProperties : NodeProperties
	{
		#region properties
		[SRCategoryAttribute(SR.Advanced)]
		[LocDisplayName(SR.BuildAction)]
		[SRDescriptionAttribute(SR.BuildActionDescription)]
		public virtual prjBuildAction BuildAction
		{
			get
			{
				string value = this.Node.ItemNode.ItemName;
				if(value == null || value.Length == 0)
				{
					return prjBuildAction.prjBuildActionNone;
				}

                KeyValuePair<string, prjBuildAction> pair = Node.ProjectManager.AvailableFileBuildActions.FirstOrDefault(i => string.Equals(i.Key, value, StringComparison.OrdinalIgnoreCase));
                return pair.Value;
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
