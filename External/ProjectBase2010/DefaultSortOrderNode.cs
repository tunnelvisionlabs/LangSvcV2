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
	/// A set of constants that specify the default sort order for different types of hierarchy nodes.
	/// </summary>
	public static class DefaultSortOrderNode
	{
		public const int HierarchyNode = 1000;
		public const int FolderNode = 500;
		public const int NestedProjectNode = 200;
		public const int ReferenceContainerNode = 300;
	}
}
