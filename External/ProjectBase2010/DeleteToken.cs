/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Storage classes for token to be deleted
	/// </summary>
	public class DeleteToken
	{
		/// <summary>
		/// String to delete
		/// </summary>
		private string token;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="token">Deletable token.</param>
		public DeleteToken(string token)
		{
			this.token = token;
		}

		/// <summary>
		/// Token marking the end of the block to delete
		/// </summary>
		public string StringToDelete
		{
			get { return token; }
		}
	}
}
