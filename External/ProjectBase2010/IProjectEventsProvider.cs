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
	/// Enable getting and setting the project events provider
	/// </summary>
	[ComVisible(true)]
	public interface IProjectEventsProvider
	{
		/// <summary>
		/// Defines the provider for the project events
		/// </summary>
		IProjectEvents ProjectEventsProvider
		{
			get;
			set;
		}
	}
}
