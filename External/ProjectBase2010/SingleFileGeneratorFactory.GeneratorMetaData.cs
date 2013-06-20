/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;

namespace Microsoft.VisualStudio.Project
{
	partial class SingleFileGeneratorFactory
	{
		private class GeneratorMetaData
		{
			#region fields
			private Guid generatorClsid = Guid.Empty;
			private int generatesDesignTimeSource = -1;
			private int generatesSharedDesignTimeSource = -1;
			private int useDesignTimeCompilationFlag = -1;
			object generator;
			#endregion

			#region ctor
			/// <summary>
			/// Constructor
			/// </summary>
			public GeneratorMetaData()
			{
			}
			#endregion

			#region Public Properties
			/// <summary>
			/// Generator instance
			/// </summary>
			public Object Generator
			{
				get
				{
					return generator;
				}
				set
				{
					generator = value;
				}
			}

			/// <summary>
			/// GeneratesDesignTimeSource reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
			/// </summary>
			public int GeneratesDesignTimeSource
			{
				get
				{
					return generatesDesignTimeSource;
				}
				set
				{
					generatesDesignTimeSource = value;
				}
			}

			/// <summary>
			/// GeneratesSharedDesignTimeSource reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
			/// </summary>
			public int GeneratesSharedDesignTimeSource
			{
				get
				{
					return generatesSharedDesignTimeSource;
				}
				set
				{
					generatesSharedDesignTimeSource = value;
				}
			}

			/// <summary>
			/// UseDesignTimeCompilationFlag reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
			/// </summary>
			public int UseDesignTimeCompilationFlag
			{
				get
				{
					return useDesignTimeCompilationFlag;
				}
				set
				{
					useDesignTimeCompilationFlag = value;
				}
			}

			/// <summary>
			/// Generator Class ID.
			/// </summary>
			public Guid GeneratorClsid
			{
				get
				{
					return generatorClsid;
				}
				set
				{
					generatorClsid = value;
				}
			}
			#endregion
		}
	}
}
