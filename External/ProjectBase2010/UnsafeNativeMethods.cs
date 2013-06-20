namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class UnsafeNativeMethods
    {
        [DllImport(ExternDll.Kernel32, SetLastError = true, EntryPoint = "RtlMoveMemory")]
        internal static extern void MoveMemory(IntPtr destination, IntPtr source, UIntPtr size);

        [DllImport(ExternDll.Kernel32, SetLastError = true)]
        internal static extern SafeGlobalAllocHandle GlobalAlloc(GlobalAllocFlags flags, UIntPtr size);

        [DllImport(ExternDll.Kernel32, SetLastError = true)]
        internal static extern IntPtr GlobalFree(IntPtr handle);

        [DllImport(ExternDll.Kernel32, SetLastError = true)]
        internal static extern IntPtr GlobalLock(SafeGlobalAllocHandle h);

        [DllImport(ExternDll.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GlobalUnlock(SafeGlobalAllocHandle h);

        [DllImport(ExternDll.Kernel32, SetLastError = true)]
        internal static extern UIntPtr GlobalSize(SafeGlobalAllocHandle h);

        [DllImport(ExternDll.Shell32, EntryPoint = "DragQueryFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern uint DragQueryFile(IntPtr hDrop, uint iFile, char[] lpszFile, uint cch);

        [DllImport(ExternDll.User32, EntryPoint = "RegisterClipboardFormatW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern ushort RegisterClipboardFormat(string format);
    }
}
