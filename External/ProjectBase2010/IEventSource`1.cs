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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Class used to identify a source of events of type SinkType.
	/// </summary>
	[ComVisible(false)]
	public interface IEventSource<SinkType>
		where SinkType : class
	{
		void OnSinkAdded(SinkType sink);
		void OnSinkRemoved(SinkType sink);
	}
}
