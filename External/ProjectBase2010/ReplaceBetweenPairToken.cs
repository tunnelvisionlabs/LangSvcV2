/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Storage classes for string to be deleted between tokens to be deleted 
	/// </summary>
	public class ReplaceBetweenPairToken
	{
		/// <summary>
		/// Token start
		/// </summary>
		private string tokenStart;

		/// <summary>
		/// End token
		/// </summary>
		private string tokenEnd;

		/// <summary>
		/// Replacement string
		/// </summary>
		private string replacement;

		/// <summary>
		/// Token identifier string
		/// </summary>
		private string tokenidentifier;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="blockStart">Start token</param>
		/// <param name="blockEnd">End Token</param>
		/// <param name="replacement">Replacement string.</param>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "tokenid")]
		public ReplaceBetweenPairToken(string tokenid, string blockStart, string blockEnd, string replacement)
		{
			tokenStart = blockStart;
			tokenEnd = blockEnd;
			this.replacement = replacement;
			tokenidentifier = tokenid;
		}

		/// <summary>
		/// Token marking the begining of the block to delete
		/// </summary>
		public string TokenStart
		{
			get { return tokenStart; }
		}

		/// <summary>
		/// Token marking the end of the block to delete
		/// </summary>
		public string TokenEnd
		{
			get { return tokenEnd; }
		}

		/// <summary>
		/// Token marking the end of the block to delete
		/// </summary>
		public string TokenReplacement
		{
			get { return replacement; }
		}

		/// <summary>
		/// Token Identifier
		/// </summary>
		public string TokenIdentifier
		{
			get { return tokenidentifier; }
		}
	}
}
