namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct _DROPFILES
    {
        public int pFiles;
        public int X;
        public int Y;
        public int fNC;
        public int fWide;
    }
}
