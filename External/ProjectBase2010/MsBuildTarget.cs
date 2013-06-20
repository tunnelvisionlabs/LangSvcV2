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
	/// Defines the constant strings for various msbuild targets
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ms")]
	public static class MsBuildTarget
	{
		public const string ResolveProjectReferences = "ResolveProjectReferences";
		public const string ResolveAssemblyReferences = "ResolveAssemblyReferences";
		public const string ResolveComReferences = "ResolveComReferences";
		public const string Build = "Build";
		public const string Rebuild = "ReBuild";
		public const string Clean = "Clean";
	}
}
