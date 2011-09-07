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
	public class FileNodeProperties : NodeProperties
	{
		#region properties
		[SRCategoryAttribute(SR.Advanced)]
		[LocDisplayName(SR.BuildAction)]
		[SRDescriptionAttribute(SR.BuildActionDescription)]
        [DefaultValue(BuildAction.None)]
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
			set
			{
				this.Node.SetEditLabel(value);
			}
		}

        [SRCategory(SR.Advanced)]
        [LocDisplayName(SR.CopyToOutputDirectory)]
        [SRDescription(SR.CopyToOutputDirectoryDescription)]
        [DefaultValue(CopyToOutputDirectoryBehavior.DoNotCopy)]
        public virtual CopyToOutputDirectoryBehavior CopyToOutputDirectory
        {
            get
            {
                if (this.Node.ItemNode.IsVirtual)
                    return CopyToOutputDirectoryBehavior.DoNotCopy;

                string metadata = this.Node.ItemNode.GetMetadata(ProjectFileConstants.CopyToOutputDirectory);
                if (string.IsNullOrEmpty(metadata))
                    return CopyToOutputDirectoryBehavior.DoNotCopy;

                return (CopyToOutputDirectoryBehavior)Enum.Parse(typeof(CopyToOutputDirectoryBehavior), metadata);
            }

            set
            {
                if (this.Node.ItemNode.Item != null)
                {
                    this.Node.ItemNode.SetMetadata(ProjectFileConstants.CopyToOutputDirectory, value.ToString());
                }
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

		#region non-browsable properties - used for automation only
		[Browsable(false)]
		public virtual string Extension
		{
			get
			{
				return Path.GetExtension(this.Node.Caption);
			}
		}
		#endregion

		#endregion

		#region ctors
		public FileNodeProperties(HierarchyNode node)
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
