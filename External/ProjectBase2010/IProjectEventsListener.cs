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
	/// Defines the interface that will specify ehethrr the object is a project events listener.
	/// </summary>
	[ComVisible(true)]
	public interface IProjectEventsListener
	{

		/// <summary>
		/// Is the object a project events listener.
		/// </summary>
		/// <returns></returns>
		bool IsProjectEventsListener
		{ get; set; }

	}
}
