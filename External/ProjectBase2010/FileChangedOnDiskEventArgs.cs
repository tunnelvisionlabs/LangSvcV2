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
	/// Event args class for triggering file change event arguments.
	/// </summary>
	public class FileChangedOnDiskEventArgs : EventArgs
	{
		#region Private fields
		/// <summary>
		/// File name that was changed on disk.
		/// </summary>
		private string fileName;

		/// <summary>
		/// The item ide of the file that has changed.
		/// </summary>
		private uint itemID;

		/// <summary>
		/// The reason the file has changed on disk.
		/// </summary>
		private _VSFILECHANGEFLAGS fileChangeFlag;
		#endregion

		/// <summary>
		/// Constructs a new event args.
		/// </summary>
		/// <param name="fileName">File name that was changed on disk.</param>
		/// <param name="id">The item id of the file that was changed on disk.</param>
		internal FileChangedOnDiskEventArgs(string fileName, uint id, _VSFILECHANGEFLAGS flag)
		{
			this.fileName = fileName;
			this.itemID = id;
			this.fileChangeFlag = flag;
		}

		/// <summary>
		/// Gets the file name that was changed on disk.
		/// </summary>
		/// <value>The file that was changed on disk.</value>
		internal string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		/// <summary>
		/// Gets item id of the file that has changed
		/// </summary>
		/// <value>The file that was changed on disk.</value>
		internal uint ItemID
		{
			get
			{
				return this.itemID;
			}
		}

		/// <summary>
		/// The reason while the file has chnaged on disk.
		/// </summary>
		/// <value>The reason while the file has chnaged on disk.</value>
		internal _VSFILECHANGEFLAGS FileChangeFlag
		{
			get
			{
				return this.fileChangeFlag;
			}
		}
	}
}
