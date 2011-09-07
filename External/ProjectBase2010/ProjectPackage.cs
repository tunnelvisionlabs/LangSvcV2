/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using ObjectExtenders = EnvDTE.ObjectExtenders;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Defines abstract package.
	/// </summary>
	[ComVisible(true)]
	[CLSCompliant(false)]
	public abstract class ProjectPackage : Microsoft.VisualStudio.Shell.Package
	{
		#region fields
		/// <summary>
		/// This is the place to register all the solution listeners.
		/// </summary>
		private List<SolutionListener> solutionListeners = new List<SolutionListener>();

        private SingleFileGeneratorNodeExtenderProvider _singleFileGeneratorNodeExtenderProvider;
        private int _singleFileGeneratorNodeExtenderCookie;

		#endregion

		#region properties
		/// <summary>
		/// Add your listener to this list. They should be added in the overridden Initialize befaore calling the base.
		/// </summary>
		protected internal IList<SolutionListener> SolutionListeners
		{
			get
			{
				return this.solutionListeners;
			}
		}

		public abstract string ProductUserContext { get; }

		#endregion

		#region methods
		protected override void Initialize()
		{
			base.Initialize();

			// Subscribe to the solution events
			this.solutionListeners.Add(new SolutionListenerForProjectReferenceUpdate(this));
			this.solutionListeners.Add(new SolutionListenerForProjectOpen(this));
			this.solutionListeners.Add(new SolutionListenerForBuildDependencyUpdate(this));
			this.solutionListeners.Add(new SolutionListenerForProjectEvents(this));

			foreach(SolutionListener solutionListener in this.solutionListeners)
			{
				solutionListener.Init();
			}

            ObjectExtenders objectExtenders = (ObjectExtenders)GetService(typeof(ObjectExtenders));
            _singleFileGeneratorNodeExtenderProvider = new SingleFileGeneratorNodeExtenderProvider();
            string extenderCatId = typeof(FileNodeProperties).GUID.ToString("B");
            string extenderName = SingleFileGeneratorNodeExtenderProvider.Name;
            string localizedName = extenderName;
            _singleFileGeneratorNodeExtenderCookie = objectExtenders.RegisterExtenderProvider(extenderCatId, extenderName, _singleFileGeneratorNodeExtenderProvider, localizedName);
		}

		protected override void Dispose(bool disposing)
		{
			// Unadvise solution listeners.
			try
			{
				if(disposing)
				{
                    ObjectExtenders objectExtenders = (ObjectExtenders)GetService(typeof(ObjectExtenders));
                    objectExtenders.UnregisterExtenderProvider(_singleFileGeneratorNodeExtenderCookie);

					foreach(SolutionListener solutionListener in this.solutionListeners)
					{
						solutionListener.Dispose();
					}

                    // Dispose the UIThread singleton.
                    UIThread.Instance.Dispose();                   
				}
			}
			finally
			{

				base.Dispose(disposing);
			}
		}
		#endregion
	}
}
