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
	/// Defines support for single file generator
	/// </summary>
	public interface ISingleFileGenerator
	{
		///<summary>
		/// Runs the generator on the item represented by the document moniker.
		/// </summary>
		/// <param name="document"></param>
		void RunGenerator(string document);
	}
}
