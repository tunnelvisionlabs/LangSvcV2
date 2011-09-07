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
	/// Used by the hierarchy node to decide which element to redraw.
	/// </summary>
	[Flags]
	[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]
	public enum UIHierarchyElement
	{
		None = 0,

		/// <summary>
		/// This will be translated to VSHPROPID_IconIndex
		/// </summary>
		Icon = 1,

		/// <summary>
		/// This will be translated to VSHPROPID_StateIconIndex
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Scc")]
		SccState = 2,

		/// <summary>
		/// This will be translated to VSHPROPID_Caption
		/// </summary>
		Caption = 4
	}
}
