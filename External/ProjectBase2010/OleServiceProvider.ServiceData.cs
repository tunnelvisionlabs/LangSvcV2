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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
	partial class OleServiceProvider
	{
		private class ServiceData : IDisposable
		{
			private Type serviceType;
			private object instance;
			private ServiceCreatorCallback creator;
			private bool shouldDispose;
			public ServiceData(Type serviceType, object instance, ServiceCreatorCallback callback, bool shouldDispose)
			{
				if(null == serviceType)
				{
					throw new ArgumentNullException("serviceType");
				}

				if((null == instance) && (null == callback))
				{
					throw new ArgumentNullException("instance");
				}

				this.serviceType = serviceType;
				this.instance = instance;
				this.creator = callback;
				this.shouldDispose = shouldDispose;
			}

			public object ServiceInstance
			{
				get
				{
					if(null == instance)
					{
						instance = creator(serviceType);
					}
					return instance;
				}
			}

			public Guid Guid
			{
				get { return serviceType.GUID; }
			}

			public void Dispose()
			{
				if((shouldDispose) && (null != instance))
				{
					IDisposable disp = instance as IDisposable;
					if(null != disp)
					{
						disp.Dispose();
					}
					instance = null;
				}
				creator = null;
				GC.SuppressFinalize(this);
			}
		}
	}
}
