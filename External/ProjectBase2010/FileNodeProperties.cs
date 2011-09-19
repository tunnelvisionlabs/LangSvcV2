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
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using prjBuildAction = VSLangProj.prjBuildAction;

	[CLSCompliant(false)]
    [ComVisible(true)]
	public class FileNodeProperties : NodeProperties
	{
		#region properties
		[SRCategoryAttribute(SR.Advanced)]
		[LocDisplayName(SR.BuildAction)]
		[SRDescriptionAttribute(SR.BuildActionDescription)]
        [DefaultValue(prjBuildAction.prjBuildActionNone)]
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
		public FileNodeProperties(FileNode node)
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
