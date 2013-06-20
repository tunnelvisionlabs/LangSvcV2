/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Project
{
	[ComVisible(true)]
	public class ProjectReferencesProperties : ReferenceNodeProperties
	{
		#region ctors
		public ProjectReferencesProperties(ProjectReferenceNode node)
			: base(node)
		{
		}
		#endregion

		#region overriden methods
		public override string FullPath
		{
			get
			{
				return ((ProjectReferenceNode)Node).ReferencedProjectOutputPath;
			}
		}
		#endregion
	}
}
