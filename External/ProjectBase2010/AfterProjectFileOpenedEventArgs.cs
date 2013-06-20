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

namespace Microsoft.VisualStudio.Project
{
	public class AfterProjectFileOpenedEventArgs : EventArgs
	{
		#region fields
		private bool added;
		#endregion

		#region properties
		/// <summary>
		/// True if the project is added to the solution after the solution is opened. false if the project is added to the solution while the solution is being opened.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal bool Added
		{
			get { return this.added; }
		}
		#endregion

		#region ctor
		internal AfterProjectFileOpenedEventArgs(bool added)
		{
			this.added = added;
		}
		#endregion
	}
}
