/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using VSLangProj;

namespace Microsoft.VisualStudio.Project.Automation
{
	/// <summary>
	/// Provides access to language-specific project events
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OAVS")]
	[ComVisible(true), CLSCompliant(false)]
	public class OAVSProjectEvents : VSProjectEvents
	{
		#region fields
		private OAVSProject vsProject;
		#endregion

		#region ctors
		public OAVSProjectEvents(OAVSProject vsProject)
		{
			this.vsProject = vsProject;
		}
		#endregion

		#region VSProjectEvents Members

		public virtual BuildManagerEvents BuildManagerEvents
		{
			get
			{
				return vsProject.BuildManager as BuildManagerEvents;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public virtual ImportsEvents ImportsEvents
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ReferencesEvents ReferencesEvents
		{
			get
			{
				// this can't return null or a NullReferenceException in Microsoft.VisualStudio.Xaml will take down the IDE (VS2010)
				ReferencesEvents events = vsProject.References as ReferencesEvents;
				return events ?? EmptyReferencesEvents.Instance;
			}
		}

		#endregion
	}
}
