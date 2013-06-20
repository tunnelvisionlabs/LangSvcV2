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

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Defines the events that are internally defined for communication with other subsytems.
	/// </summary>
	[ComVisible(true)]
	public interface IProjectEvents
	{
		/// <summary>
		/// Event raised just after the project file opened.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix")]
		event EventHandler<AfterProjectFileOpenedEventArgs> AfterProjectFileOpened;

		/// <summary>
		/// Event raised before the project file closed.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix")]
		event EventHandler<BeforeProjectFileClosedEventArgs> BeforeProjectFileClosed;
	}
}
