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
	/// <summary>
	/// Defines the status of the command being queried
	/// </summary>
	[Flags]
	[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
	public enum QueryStatusResult
	{
		/// <summary>
		/// The command is not supported.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NOTSUPPORTED")]
		NOTSUPPORTED = 0,

		/// <summary>
		/// The command is supported
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SUPPORTED")]
		SUPPORTED = 1,

		/// <summary>
		/// The command is enabled
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ENABLED")]
		ENABLED = 2,

		/// <summary>
		/// The command is toggled on
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "LATCHED")]
		LATCHED = 4,

		/// <summary>
		/// The command is toggled off (the opposite of LATCHED).
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NINCHED")]
		NINCHED = 8,

		/// <summary>
		/// The command is invisible.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INVISIBLE")]
		INVISIBLE = 16
	}
}
