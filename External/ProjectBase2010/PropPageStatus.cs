/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Defines the currect state of a property page.
	/// </summary>
	[Flags]
	public enum PropPageStatus
	{

		Dirty = 0x1,

		Validate = 0x2,

		Clean = 0x4
	}
}
