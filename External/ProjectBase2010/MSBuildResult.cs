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
	/// Defines the current status of the build process.
	/// </summary>
	public enum MSBuildResult
	{
		/// <summary>
		/// The build is currently suspended.
		/// </summary>
		Suspended,

		/// <summary>
		/// The build has been restarted.
		/// </summary>
		Resumed,

		/// <summary>
		/// The build failed.
		/// </summary>
		Failed,

		/// <summary>
		/// The build was successful.
		/// </summary>
		Successful,
	}
}
