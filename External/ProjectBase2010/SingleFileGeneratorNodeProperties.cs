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
	public class SingleFileGeneratorNodeProperties : FileNodeProperties
	{
		#region fields
		private EventHandler<HierarchyNodeEventArgs> onCustomToolChanged;
		private EventHandler<HierarchyNodeEventArgs> onCustomToolNameSpaceChanged;
		#endregion

		#region custom tool events
		internal event EventHandler<HierarchyNodeEventArgs> OnCustomToolChanged
		{
			add { onCustomToolChanged += value; }
			remove { onCustomToolChanged -= value; }
		}

		internal event EventHandler<HierarchyNodeEventArgs> OnCustomToolNameSpaceChanged
		{
			add { onCustomToolNameSpaceChanged += value; }
			remove { onCustomToolNameSpaceChanged -= value; }
		}

		#endregion

		#region properties
		[SRCategoryAttribute(SR.Advanced)]
		[LocDisplayName(SR.CustomTool)]
		[SRDescriptionAttribute(SR.CustomToolDescription)]
		public virtual string CustomTool
		{
			get
			{
				return this.Node.ItemNode.GetMetadata(ProjectFileConstants.Generator);
			}
			set
			{
				if (CustomTool != value)
				{
					this.Node.ItemNode.SetMetadata(ProjectFileConstants.Generator, value != string.Empty ? value : null);
					HierarchyNodeEventArgs args = new HierarchyNodeEventArgs(this.Node);
					if (onCustomToolChanged != null)
					{
						onCustomToolChanged(this.Node, args);
					}
				}
			}
		}

		[SRCategoryAttribute(VisualStudio.Project.SR.Advanced)]
		[LocDisplayName(SR.CustomToolNamespace)]
		[SRDescriptionAttribute(SR.CustomToolNamespaceDescription)]
		public virtual string CustomToolNamespace
		{
			get
			{
				return this.Node.ItemNode.GetMetadata(ProjectFileConstants.CustomToolNamespace);
			}
			set
			{
				if (CustomToolNamespace != value)
				{
					this.Node.ItemNode.SetMetadata(ProjectFileConstants.CustomToolNamespace, value != String.Empty ? value : null);
					HierarchyNodeEventArgs args = new HierarchyNodeEventArgs(this.Node);
					if (onCustomToolNameSpaceChanged != null)
					{
						onCustomToolNameSpaceChanged(this.Node, args);
					}
				}
			}
		}
		#endregion

		#region ctors
		public SingleFileGeneratorNodeProperties(HierarchyNode node)
			: base(node)
		{
		}
		#endregion
	}
}
