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
	///  Storage classes for replacement tokens
	/// </summary>
	public class ReplacePairToken
	{
		/// <summary>
		/// token string
		/// </summary>
		private string token;

		/// <summary>
		/// Replacement string
		/// </summary>
		private string replacement;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="token">replaceable token</param>
		/// <param name="replacement">replacement string</param>
		public ReplacePairToken(string token, string replacement)
		{
			this.token = token;
			this.replacement = replacement;
		}

		/// <summary>
		/// Token that needs to be replaced
		/// </summary>
		public string Token
		{
			get { return token; }
		}
		/// <summary>
		/// String to replace the token with
		/// </summary>
		public string Replacement
		{
			get { return replacement; }
		}
	}
}
