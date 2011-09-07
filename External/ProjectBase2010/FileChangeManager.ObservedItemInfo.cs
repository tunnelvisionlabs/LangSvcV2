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
using System.Globalization;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = System.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// This object is in charge of reloading nodes that have file monikers that can be listened to changes
	/// </summary>
	partial class FileChangeManager
	{
		/// <summary>
		/// Defines a data structure that can link a item moniker to the item and its file change cookie.
		/// </summary>
		private struct ObservedItemInfo
		{
			/// <summary>
			/// Defines the id of the item that is to be reloaded.
			/// </summary>
			private uint itemID;

			/// <summary>
			/// Defines the file change cookie that is returned when listening on file changes on the nested project item.
			/// </summary>
			private uint fileChangeCookie;

			/// <summary>
			/// Defines the nested project item that is to be reloaded.
			/// </summary>
			internal uint ItemID
			{
				get
				{
					return this.itemID;
				}

				set
				{
					this.itemID = value;
				}
			}

			/// <summary>
			/// Defines the file change cookie that is returned when listenning on file changes on the nested project item.
			/// </summary>
			internal uint FileChangeCookie
			{
				get
				{
					return this.fileChangeCookie;
				}

				set
				{
					this.fileChangeCookie = value;
				}
			}
		}
	}
}
