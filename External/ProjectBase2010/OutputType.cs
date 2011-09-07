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
	/// Defines possible types of output that can produced by a language project
	/// </summary>
	[PropertyPageTypeConverterAttribute(typeof(OutputTypeConverter))]
	public enum OutputType
	{
		/// <summary>
		/// The output type is a class library.
		/// </summary>
		Library,

		/// <summary>
		/// The output type is a windows executable.
		/// </summary>
		WinExe,

		/// <summary>
		/// The output type is an executable.
		/// </summary>
		Exe
	}
}
