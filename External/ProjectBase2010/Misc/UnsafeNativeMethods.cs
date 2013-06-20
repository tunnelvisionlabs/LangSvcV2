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
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

	internal static partial class UnsafeNativeMethods
	{
		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleSetClipboard(Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject);

		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleGetClipboard(out Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject);

		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleFlushClipboard();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OpenClipboard(IntPtr newOwner);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int EmptyClipboard();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int CloseClipboard();

		[DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
		internal static extern int ImageList_GetImageCount(HandleRef himl);

		[DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
		internal static extern bool ImageList_Draw(HandleRef himl, int i, HandleRef hdcDst, int x, int y, int fStyle);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), DllImport(ExternDll.Shell32, EntryPoint = "SHGetSpecialFolderLocation")]
		internal static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] ppidl);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), DllImport(ExternDll.Shell32, EntryPoint = "SHGetPathFromIDList", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		internal static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);
	}
}

