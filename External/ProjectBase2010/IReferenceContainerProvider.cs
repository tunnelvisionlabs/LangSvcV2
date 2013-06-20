/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Provides access to the reference data container.
	/// </summary>
	/// <remarks>Normally this should be an internal interface but since it should be available for
	/// the aggregator it must be made public.</remarks>
	[ComVisible(true)]
	public interface IReferenceContainerProvider
	{
		IReferenceContainer GetReferenceContainer();
	}
}
