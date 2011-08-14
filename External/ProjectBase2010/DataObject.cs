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
	internal enum tagDVASPECT
	{
		DVASPECT_CONTENT = 1,
		DVASPECT_THUMBNAIL = 2,
		DVASPECT_ICON = 4,
		DVASPECT_DOCPRINT = 8
	}

	internal enum tagTYMED
	{
		TYMED_HGLOBAL = 1,
		TYMED_FILE = 2,
		TYMED_ISTREAM = 4,
		TYMED_ISTORAGE = 8,
		TYMED_GDI = 16,
		TYMED_MFPICT = 32,
		TYMED_ENHMF = 64,
		TYMED_NULL = 0
	}

	internal sealed class DataCacheEntry : IDisposable
	{
		#region fields
		/// <summary>
		/// Defines an object that will be a mutex for this object for synchronizing thread calls.
		/// </summary>
		private static volatile object Mutex = new object();

		private FORMATETC format;

		private SafeGlobalAllocHandle data;

		private DATADIR dataDir;
		#endregion

		#region properties
		internal FORMATETC Format
		{
			get
			{
				return this.format;
			}
		}

		internal SafeGlobalAllocHandle Data
		{
			get
			{
				return this.data;
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal DATADIR DataDir
		{
			get
			{
				return this.dataDir;
			}
		}

		#endregion

		/// <summary>
		/// The IntPtr is data allocated that should be removed. It is allocated by the ProcessSelectionData method.
		/// </summary>
		internal DataCacheEntry(FORMATETC fmt, SafeGlobalAllocHandle data, DATADIR dir)
		{
			this.format = fmt;
			this.data = data;
			this.dataDir = dir;
		}

		#region Dispose
		/// <summary>
		/// The IDispose interface Dispose method for disposing the object determinastically.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// The method that does the cleanup.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				SafeGlobalAllocHandle handle = this.data;
				if (handle != null)
				{
					handle.Dispose();
					data = null;
				}
			}
		}
		#endregion
	}

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

	[SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public static class DragDropHelper
	{
#pragma warning disable 414
		public static readonly ushort CF_VSREFPROJECTITEMS;
		public static readonly ushort CF_VSSTGPROJECTITEMS;
		public static readonly ushort CF_VSPROJECTCLIPDESCRIPTOR;
#pragma warning restore 414

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static DragDropHelper()
		{
			CF_VSREFPROJECTITEMS = UnsafeNativeMethods.RegisterClipboardFormat("CF_VSREFPROJECTITEMS");
			CF_VSSTGPROJECTITEMS = UnsafeNativeMethods.RegisterClipboardFormat("CF_VSSTGPROJECTITEMS");
			CF_VSPROJECTCLIPDESCRIPTOR = UnsafeNativeMethods.RegisterClipboardFormat("CF_PROJECTCLIPBOARDDESCRIPTOR");
		}


		public static FORMATETC CreateFormatEtc(ushort iFormat)
		{
			FORMATETC fmt = new FORMATETC();
			fmt.cfFormat = iFormat;
			fmt.ptd = IntPtr.Zero;
			fmt.dwAspect = (uint)DVASPECT.DVASPECT_CONTENT;
			fmt.lindex = -1;
			fmt.tymed = (uint)TYMED.TYMED_HGLOBAL;
			return fmt;
		}

		public static int QueryGetData(Microsoft.VisualStudio.OLE.Interop.IDataObject pDataObject, ref FORMATETC fmtetc)
		{
			int returnValue = VSConstants.E_FAIL;
			FORMATETC[] af = new FORMATETC[1];
			af[0] = fmtetc;
			try
			{
				int result = ErrorHandler.ThrowOnFailure(pDataObject.QueryGetData(af));
				if(result == VSConstants.S_OK)
				{
					fmtetc = af[0];
					returnValue = VSConstants.S_OK;
				}
			}
			catch(COMException e)
			{
				Trace.WriteLine("COMException : " + e.Message);
				returnValue = e.ErrorCode;
			}

			return returnValue;
		}

		public static STGMEDIUM GetData(Microsoft.VisualStudio.OLE.Interop.IDataObject pDataObject, ref FORMATETC fmtetc)
		{
			FORMATETC[] af = new FORMATETC[1];
			af[0] = fmtetc;
			STGMEDIUM[] sm = new STGMEDIUM[1];
			pDataObject.GetData(af, sm);
			fmtetc = af[0];
			return sm[0];
		}

		/// <summary>
		/// Retrives data from a VS format.
		/// </summary>
		public static List<string> GetDroppedFiles(ushort format, Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject, out DropDataType ddt)
		{
			ddt = DropDataType.None;
			List<string> droppedFiles = new List<string>();

			// try HDROP
			FORMATETC fmtetc = CreateFormatEtc(format);

			if(QueryGetData(dataObject, ref fmtetc) == VSConstants.S_OK)
			{
				STGMEDIUM stgmedium = DragDropHelper.GetData(dataObject, ref fmtetc);
				if(stgmedium.tymed == (uint)TYMED.TYMED_HGLOBAL)
				{
					// We are releasing the cloned hglobal here.
					IntPtr dropInfoHandle = stgmedium.unionmember;
					if(dropInfoHandle != IntPtr.Zero)
					{
						ddt = DropDataType.Shell;
						try
						{
							uint numFiles = UnsafeNativeMethods.DragQueryFile(dropInfoHandle, 0xFFFFFFFF, null, 0);

							// We are a directory based project thus a projref string is placed on the clipboard.
							// We assign the maximum length of a projref string.
							// The format of a projref is : <Proj Guid>|<project rel path>|<file path>
							uint lenght = (uint)Guid.Empty.ToString().Length + 2 * NativeMethods.MAX_PATH + 2;
							char[] moniker = new char[lenght + 1];
							for(uint fileIndex = 0; fileIndex < numFiles; fileIndex++)
							{
								uint queryFileLength = UnsafeNativeMethods.DragQueryFile(dropInfoHandle, fileIndex, moniker, lenght);
								string filename = new String(moniker, 0, (int)queryFileLength);
								droppedFiles.Add(filename);
							}
						}
						finally
						{
							Marshal.FreeHGlobal(dropInfoHandle);
						}
					}
				}
			}

			return droppedFiles;
		}

		public static string GetSourceProjectPath(Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject)
		{
			string projectPath = null;
			FORMATETC fmtetc = CreateFormatEtc(CF_VSPROJECTCLIPDESCRIPTOR);

			if(QueryGetData(dataObject, ref fmtetc) == VSConstants.S_OK)
			{
				STGMEDIUM stgmedium = DragDropHelper.GetData(dataObject, ref fmtetc);
				if(stgmedium.tymed == (uint)TYMED.TYMED_HGLOBAL && stgmedium.unionmember != IntPtr.Zero)
				{
					// We are releasing the cloned hglobal here.
					using (SafeGlobalAllocHandle dropInfoHandle = new SafeGlobalAllocHandle(stgmedium.unionmember, true))
					{
						projectPath = GetData(dropInfoHandle);
					}
				}
			}

			return projectPath;
		}

		/// <summary>
		/// Returns the data packed after the DROPFILES structure.
		/// </summary>
		/// <param name="dropHandle"></param>
		/// <returns></returns>
		internal static string GetData(SafeGlobalAllocHandle dropHandle)
		{
			IntPtr data = UnsafeNativeMethods.GlobalLock(dropHandle);
			try
			{
				_DROPFILES df = (_DROPFILES)Marshal.PtrToStructure(data, typeof(_DROPFILES));
				if(df.fWide != 0)
				{
					IntPtr pdata = new IntPtr((long)data + df.pFiles);
					return Marshal.PtrToStringUni(pdata);
				}
			}
			finally
			{
				if(data != null)
				{
					UnsafeNativeMethods.GlobalUnlock(dropHandle);
				}
			}

			return null;
		}

		internal static SafeGlobalAllocHandle CopyHGlobal(SafeGlobalAllocHandle data)
		{
			IntPtr src = UnsafeNativeMethods.GlobalLock(data);
			UIntPtr size = UnsafeNativeMethods.GlobalSize(data);
			SafeGlobalAllocHandle ptr = UnsafeNativeMethods.GlobalAlloc(0, size);
			IntPtr buffer = UnsafeNativeMethods.GlobalLock(ptr);

			try
			{
				UnsafeNativeMethods.MoveMemory(buffer, src, size);
			}
			finally
			{
				if(buffer != IntPtr.Zero)
				{
					UnsafeNativeMethods.GlobalUnlock(ptr);
				}

				if(src != IntPtr.Zero)
				{
					UnsafeNativeMethods.GlobalUnlock(data);
				}
			}

			return ptr;
		}

		public static void CopyStringToHGlobal(string s, IntPtr data, int bufferSize)
		{
			if (s == null)
				throw new ArgumentNullException("s");
			if (data == null)
				throw new ArgumentNullException("data");
			if (bufferSize < 0)
				throw new ArgumentOutOfRangeException("bufferSize");

			byte[] stringData = System.Text.Encoding.Unicode.GetBytes(s);
			if (bufferSize < stringData.Length + 2)
				throw new ArgumentException();

			Marshal.Copy(stringData, 0, data, stringData.Length);
			Marshal.WriteInt16(data, stringData.Length, 0);
		}

	} // end of dragdrophelper

	internal class EnumSTATDATA : IEnumSTATDATA
	{
		IEnumerable i;

		IEnumerator e;

		public EnumSTATDATA(IEnumerable i)
		{
			this.i = i;
			this.e = i.GetEnumerator();
		}

		void IEnumSTATDATA.Clone(out IEnumSTATDATA clone)
		{
			clone = new EnumSTATDATA(i);
		}

		int IEnumSTATDATA.Next(uint celt, STATDATA[] d, out uint fetched)
		{
			uint rc = 0;
			//uint size = (fetched != null) ? fetched[0] : 0;
			for(uint i = 0; i < celt; i++)
			{
				if(e.MoveNext())
				{
					STATDATA sdata = (STATDATA)e.Current;

					rc++;
					if(d != null && d.Length > i)
					{
						d[i] = sdata;
					}
				}
			}

			fetched = rc;
			return 0;
		}

		int IEnumSTATDATA.Reset()
		{
			e.Reset();
			return 0;
		}

		int IEnumSTATDATA.Skip(uint celt)
		{
			for(uint i = 0; i < celt; i++)
			{
				e.MoveNext();
			}

			return 0;
		}
	}

	internal class EnumFORMATETC : IEnumFORMATETC
	{
		IEnumerable cache; // of DataCacheEntrys.

		DATADIR dir;

		IEnumerator e;

		public EnumFORMATETC(DATADIR dir, IEnumerable cache)
		{
			this.cache = cache;
			this.dir = dir;
			e = cache.GetEnumerator();
		}

		void IEnumFORMATETC.Clone(out IEnumFORMATETC clone)
		{
			clone = new EnumFORMATETC(dir, cache);
		}

		int IEnumFORMATETC.Next(uint celt, FORMATETC[] d, uint[] fetched)
		{
			uint rc = 0;
			//uint size = (fetched != null) ? fetched[0] : 0;
			for(uint i = 0; i < celt; i++)
			{
				if(e.MoveNext())
				{
					DataCacheEntry entry = (DataCacheEntry)e.Current;

					rc++;
					if(d != null && d.Length > i)
					{
						d[i] = entry.Format;
					}
				}
				else
				{
					return VSConstants.S_FALSE;
				}
			}

			if(fetched != null && fetched.Length > 0)
				fetched[0] = rc;
			return VSConstants.S_OK;
		}

		int IEnumFORMATETC.Reset()
		{
			e.Reset();
			return 0;
		}

		int IEnumFORMATETC.Skip(uint celt)
		{
			for(uint i = 0; i < celt; i++)
			{
				e.MoveNext();
			}

			return 0;
		}
	}
}
