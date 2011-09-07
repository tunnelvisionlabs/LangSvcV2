/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// This class is used for the events raised by a HierarchyNode object.
	/// </summary>
	public class HierarchyNodeEventArgs : EventArgs
	{
		private HierarchyNode child;

		internal HierarchyNodeEventArgs(HierarchyNode child)
		{
			this.child = child;
		}

		public HierarchyNode Child
		{
			get { return this.child; }
		}
	}
}
