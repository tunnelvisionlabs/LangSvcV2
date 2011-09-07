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
	/// An enumeration that describes the type of action to be taken by the build.
	/// </summary>
	[PropertyPageTypeConverterAttribute(typeof(BuildActionConverter))]
	public enum BuildAction
	{
		None,
		Compile,
		Content,
		EmbeddedResource,
        Localization,
        Config,
        ConfigTemplate
	}
}
