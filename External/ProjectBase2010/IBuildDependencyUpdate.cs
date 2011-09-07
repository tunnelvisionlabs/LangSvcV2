/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using MSBuild = Microsoft.Build.Evaluation;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Interface for manipulating build dependency
	/// </summary>
	/// <remarks>Normally this should be an internal interface but since it shouldbe available for the aggregator it must be made public.</remarks>
	[ComVisible(true)]
	[CLSCompliant(false)]
	public interface IBuildDependencyUpdate
	{
		/// <summary>
		/// Defines a container for storing BuildDependencies
		/// </summary>

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		IVsBuildDependency[] BuildDependencies
		{
			get;
		}

		/// <summary>
		/// Adds a BuildDependency to the container
		/// </summary>
		/// <param name="dependency">The dependency to add</param>
		void AddBuildDependency(IVsBuildDependency dependency);

		/// <summary>
		/// Removes the builddependency from teh container.
		/// </summary>
		/// <param name="dependency">The dependency to add</param>
		void RemoveBuildDependency(IVsBuildDependency dependency);

	}
}
