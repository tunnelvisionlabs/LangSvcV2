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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.Project
{
	/// <summary>
	/// Unfortunately System.Windows.Forms.IDataObject and
	/// Microsoft.VisualStudio.OLE.Interop.IDataObject are different...
	/// </summary>
	public sealed class DataObject : IDataObject
	{
		#region fields
		internal const int DATA_S_SAMEFORMATETC = 0x00040130;
		EventSinkCollection map;
		ArrayList entries;
		#endregion

		public DataObject()
		{
			this.map = new EventSinkCollection();
			this.entries = new ArrayList();
		}

		public void SetData(FORMATETC format, SafeGlobalAllocHandle data)
		{
			this.entries.Add(new DataCacheEntry(format, data, DATADIR.DATADIR_SET));
		}

		#region IDataObject methods
		int IDataObject.DAdvise(FORMATETC[] e, uint adv, IAdviseSink sink, out uint cookie)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}

			STATDATA sdata = new STATDATA();

			sdata.ADVF = adv;
			sdata.FORMATETC = e[0];
			sdata.pAdvSink = sink;
			cookie = this.map.Add(sdata);
			sdata.dwConnection = cookie;
			return 0;
		}

		void IDataObject.DUnadvise(uint cookie)
		{
			this.map.RemoveAt(cookie);
		}

		int IDataObject.EnumDAdvise(out IEnumSTATDATA e)
		{
			e = new EnumSTATDATA((IEnumerable)this.map);
			return 0; //??
		}

		int IDataObject.EnumFormatEtc(uint direction, out IEnumFORMATETC penum)
		{
			penum = new EnumFORMATETC((DATADIR)direction, (IEnumerable)this.entries);
			return 0;
		}

		int IDataObject.GetCanonicalFormatEtc(FORMATETC[] format, FORMATETC[] fmt)
		{
			throw new System.Runtime.InteropServices.COMException("", DATA_S_SAMEFORMATETC);
		}

		void IDataObject.GetData(FORMATETC[] fmt, STGMEDIUM[] m)
		{
			STGMEDIUM retMedium = new STGMEDIUM();

			if(fmt == null || fmt.Length < 1)
				return;

			SafeGlobalAllocHandle copy = null;
			foreach(DataCacheEntry e in this.entries)
			{
				if(e.Format.cfFormat == fmt[0].cfFormat /*|| fmt[0].cfFormat == InternalNativeMethods.CF_HDROP*/)
				{
					retMedium.tymed = e.Format.tymed;

					// Caller must delete the memory.
					copy = DragDropHelper.CopyHGlobal(e.Data);
					retMedium.unionmember = copy.DangerousGetHandle();
					break;
				}
			}

			if (m != null && m.Length > 0)
			{
				m[0] = retMedium;
				if (copy != null)
					copy.SetHandleAsInvalid();
			}
		}

		void IDataObject.GetDataHere(FORMATETC[] fmt, STGMEDIUM[] m)
		{
		}

		int IDataObject.QueryGetData(FORMATETC[] fmt)
		{
			if(fmt == null || fmt.Length < 1)
				return VSConstants.S_FALSE;

			foreach(DataCacheEntry e in this.entries)
			{
				if(e.Format.cfFormat == fmt[0].cfFormat /*|| fmt[0].cfFormat == InternalNativeMethods.CF_HDROP*/)
					return VSConstants.S_OK;
			}

			return VSConstants.S_FALSE;
		}

		void IDataObject.SetData(FORMATETC[] fmt, STGMEDIUM[] m, int fRelease)
		{
		}
		#endregion
	}
}
