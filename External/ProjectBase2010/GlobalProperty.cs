/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Defines the global propeties used by the msbuild project.
	/// </summary>
	public enum GlobalProperty
	{
		/// <summary>
		/// Property specifying that we are building inside VS.
		/// </summary>
		BuildingInsideVisualStudio,

		/// <summary>
		/// The VS installation directory. This is the same as the $(DevEnvDir) macro.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Env")]
		DevEnvDir,

		/// <summary>
		/// The name of the solution the project is created. This is the same as the $(SolutionName) macro.
		/// </summary>
		SolutionName,

		/// <summary>
		/// The file name of the solution. This is the same as $(SolutionFileName) macro.
		/// </summary>
		SolutionFileName,

		/// <summary>
		/// The full path of the solution. This is the same as the $(SolutionPath) macro.
		/// </summary>
		SolutionPath,

		/// <summary>
		/// The directory of the solution. This is the same as the $(SolutionDir) macro.
		/// </summary>               
		SolutionDir,

		/// <summary>
		/// The extension of teh directory. This is the same as the $(SolutionExt) macro.
		/// </summary>
		SolutionExt,

		/// <summary>
		/// The fxcop installation directory.
		/// </summary>
		FxCopDir,

		/// <summary>
		/// The ResolvedNonMSBuildProjectOutputs msbuild property
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "VSIDE")]
		VSIDEResolvedNonMSBuildProjectOutputs,

		/// <summary>
		/// The Configuartion property.
		/// </summary>
		Configuration,

		/// <summary>
		/// The platform property.
		/// </summary>
		Platform,

		/// <summary>
		/// The RunCodeAnalysisOnce property
		/// </summary>
		RunCodeAnalysisOnce,

        /// <summary>
        /// The VisualStudioStyleErrors property
        /// </summary>
        VisualStudioStyleErrors,
	}
}
