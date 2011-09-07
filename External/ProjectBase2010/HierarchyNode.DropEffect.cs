/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
//#define CCI_TRACING
using Microsoft.VisualStudio.Shell.Interop;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace Microsoft.VisualStudio.Project
{
	partial class HierarchyNode
	{
		/// <summary>
		/// DropEffect as defined in oleidl.h
		/// </summary>
		public enum DropEffect
		{
			None,
			Copy = 1,
			Move = 2,
			Link = 4
		}
	}
}
