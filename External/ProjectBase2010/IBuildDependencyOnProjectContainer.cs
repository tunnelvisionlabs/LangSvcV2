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
	/// This interface defines the rules for handling build dependency on a project container.
	/// </summary>
	/// <remarks>Normally this should be an internal interface but since it shouldbe available for the aggregator it must be made public.</remarks>
	[ComVisible(true)]
	[CLSCompliant(false)]
	public interface IBuildDependencyOnProjectContainer
	{
		/// <summary>
		/// Defines whether the nested projects should be build with the parent project.
		/// </summary>
		bool BuildNestedProjectsOnBuild
		{
			get;
			set;
		}

		/// <summary>
		/// Enumerates the nested hierachies present that will participate in the build dependency update.
		/// </summary>
		/// <returns>A list of hierrachies.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hierachies")]
		IVsHierarchy[] EnumNestedHierachiesForBuildDependency();
	}
}
