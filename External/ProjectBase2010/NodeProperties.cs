/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// To create your own localizable node properties, subclass this and add public properties
	/// decorated with your own localized display name, category and description attributes.
	/// </summary>
	[CLSCompliant(false), ComVisible(true)]
	public class NodeProperties : LocalizableProperties,
		ISpecifyPropertyPages,
		IVsGetCfgProvider,
		IVsSpecifyProjectDesignerPages,
		EnvDTE80.IInternalExtenderProvider,
		IVsBrowseObject
	{
		#region fields
		private HierarchyNode node;
		#endregion

		#region properties
		[Browsable(false)]
		[AutomationBrowsable(false)]
		public HierarchyNode Node
		{
			get { return this.node; }
		}

		/// <summary>
		/// Used by Property Pages Frame to set it's title bar. The Caption of the Hierarchy Node is returned.
		/// </summary>
		[Browsable(false)]
		[AutomationBrowsable(false)]
		public virtual string Name
		{
			get { return this.node.Caption; }
		}

		#endregion

		#region ctors
		public NodeProperties(HierarchyNode node)
		{
			if(node == null)
			{
				throw new ArgumentNullException("node");
			}
			this.node = node;
		}
		#endregion

		#region ISpecifyPropertyPages methods
		public virtual void GetPages(CAUUID[] pages)
		{
			this.GetCommonPropertyPages(pages);
		}
		#endregion

		#region IVsSpecifyProjectDesignerPages
		/// <summary>
		/// Implementation of the IVsSpecifyProjectDesignerPages. It will retun the pages that are configuration independent.
		/// </summary>
		/// <param name="pages">The pages to return.</param>
		/// <returns></returns>
		public virtual int GetProjectDesignerPages(CAUUID[] pages)
		{
			this.GetCommonPropertyPages(pages);
			return VSConstants.S_OK;
		}
		#endregion

		#region IVsGetCfgProvider methods
		public virtual int GetCfgProvider(out IVsCfgProvider p)
		{
			p = null;
			return VSConstants.E_NOTIMPL;
		}
		#endregion

		#region IVsBrowseObject methods
		/// <summary>
		/// Maps back to the hierarchy or project item object corresponding to the browse object.
		/// </summary>
		/// <param name="hier">Reference to the hierarchy object.</param>
		/// <param name="itemid">Reference to the project item.</param>
		/// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
		public virtual int GetProjectItem(out IVsHierarchy hier, out uint itemid)
		{
			if(this.node == null)
			{
				throw new InvalidOperationException();
			}
			hier = HierarchyNode.GetOuterHierarchy(this.node.ProjectManager);
			itemid = this.node.ID;
			return VSConstants.S_OK;
		}
		#endregion

		#region overridden methods
		/// <summary>
		/// Get the Caption of the Hierarchy Node instance. If Caption is null or empty we delegate to base
		/// </summary>
		/// <returns>Caption of Hierarchy node instance</returns>
		public override string GetComponentName()
		{
			string caption = this.Node.Caption;
			if(string.IsNullOrEmpty(caption))
			{
				return base.GetComponentName();
			}
			else
			{
				return caption;
			}
		}
		#endregion

		#region helper methods
		protected string GetProperty(string name, string def)
		{
			string a = this.Node.ItemNode.GetMetadata(name);
			return (a == null) ? def : a;
		}

		protected void SetProperty(string name, string value)
		{
			this.Node.ItemNode.SetMetadata(name, value);
		}

		/// <summary>
		/// Retrieves the common property pages. The NodeProperties is the BrowseObject and that will be called to support 
		/// configuration independent properties.
		/// </summary>
		/// <param name="pages">The pages to return.</param>
		private void GetCommonPropertyPages(CAUUID[] pages)
		{
			// We do not check whether the supportsProjectDesigner is set to false on the ProjectNode.
			// We rely that the caller knows what to call on us.
			if(pages == null)
			{
				throw new ArgumentNullException("pages");
			}

			if(pages.Length == 0)
			{
				throw new ArgumentException(SR.GetString(SR.InvalidParameter, CultureInfo.CurrentUICulture), "pages");
			}

			// Only the project should show the property page the rest should show the project properties.
			if(this.node != null && (this.node is ProjectNode))
			{
				// Retrieve the list of guids from hierarchy properties.
				// Because a flavor could modify that list we must make sure we are calling the outer most implementation of IVsHierarchy
				string guidsList = String.Empty;
				IVsHierarchy hierarchy = HierarchyNode.GetOuterHierarchy(this.Node.ProjectManager);
				object variant = null;
				ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList, out variant));
				guidsList = (string)variant;

				Guid[] guids = Utilities.GuidsArrayFromSemicolonDelimitedStringOfGuids(guidsList);
				if(guids == null || guids.Length == 0)
				{
					pages[0] = new CAUUID();
					pages[0].cElems = 0;
				}
				else
				{
					pages[0] = PackageUtilities.CreateCAUUIDFromGuidArray(guids);
				}
			}
			else
			{
				pages[0] = new CAUUID();
				pages[0].cElems = 0;
			}
		}
		#endregion

		#region IInternalExtenderProvider Members

		bool EnvDTE80.IInternalExtenderProvider.CanExtend(string extenderCATID, string extenderName, object extendeeObject)
		{
			EnvDTE80.IInternalExtenderProvider outerHierarchy = HierarchyNode.GetOuterHierarchy(this.Node) as EnvDTE80.IInternalExtenderProvider;


			if(outerHierarchy != null)
			{
				return outerHierarchy.CanExtend(extenderCATID, extenderName, extendeeObject);
			}
			return false;
		}

		object EnvDTE80.IInternalExtenderProvider.GetExtender(string extenderCATID, string extenderName, object extendeeObject, EnvDTE.IExtenderSite extenderSite, int cookie)
		{
			EnvDTE80.IInternalExtenderProvider outerHierarchy = HierarchyNode.GetOuterHierarchy(this.Node) as EnvDTE80.IInternalExtenderProvider;

			if(outerHierarchy != null)
			{
				return outerHierarchy.GetExtender(extenderCATID, extenderName, extendeeObject, extenderSite, cookie);
			}

			return null;
		}

		object EnvDTE80.IInternalExtenderProvider.GetExtenderNames(string extenderCATID, object extendeeObject)
		{
			EnvDTE80.IInternalExtenderProvider outerHierarchy = HierarchyNode.GetOuterHierarchy(this.Node) as EnvDTE80.IInternalExtenderProvider;

			if(outerHierarchy != null)
			{
				return outerHierarchy.GetExtenderNames(extenderCATID, extendeeObject);
			}

			return null;
		}

		#endregion

		#region ExtenderSupport
		[Browsable(false)]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CATID")]
		public virtual string ExtenderCATID
		{
			get
			{
				Guid catid = this.Node.ProjectManager.GetCATIDForType(this.GetType());
				if(Guid.Empty.CompareTo(catid) == 0)
				{
					return null;
				}
				return catid.ToString("B");
			}
		}

		[Browsable(false)]
		public object ExtenderNames()
		{
			EnvDTE.ObjectExtenders extenderService = (EnvDTE.ObjectExtenders)this.Node.GetService(typeof(EnvDTE.ObjectExtenders));
			Debug.Assert(extenderService != null, "Could not get the ObjectExtenders object from the services exposed by this property object");
			if(extenderService == null)
			{
				throw new InvalidOperationException();
			}
			return extenderService.GetExtenderNames(this.ExtenderCATID, this);
		}

		public object Extender(string extenderName)
		{
			EnvDTE.ObjectExtenders extenderService = (EnvDTE.ObjectExtenders)this.Node.GetService(typeof(EnvDTE.ObjectExtenders));
			Debug.Assert(extenderService != null, "Could not get the ObjectExtenders object from the services exposed by this property object");
			if(extenderService == null)
			{
				throw new InvalidOperationException();
			}
			return extenderService.GetExtender(this.ExtenderCATID, extenderName, this);
		}

		#endregion
	}
}
