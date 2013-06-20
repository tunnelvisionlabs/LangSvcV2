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
	/// <summary>
	/// All public properties on Nodeproperties or derived classes are assumed to be used by Automation by default.
	/// Set this attribute to false on Properties that should not be visible for Automation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class AutomationBrowsableAttribute : System.Attribute
	{
		public AutomationBrowsableAttribute(bool browsable)
		{
			this.browsable = browsable;
		}

		public bool Browsable
		{
			get
			{
				return this.browsable;
			}
		}

		private bool browsable;
	}
}
