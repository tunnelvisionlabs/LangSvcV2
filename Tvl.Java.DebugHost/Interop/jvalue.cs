namespace Tvl.Java.DebugHost.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    public struct jvalue
    {
        [FieldOffset(0)]
        public byte ByteValue;

        [FieldOffset(0)]
        public char CharValue;

        [FieldOffset(0)]
        public short ShortValue;

        [FieldOffset(0)]
        public int IntValue;

        [FieldOffset(0)]
        public long LongValue;

        [FieldOffset(0)]
        public float FloatValue;

        [FieldOffset(0)]
        public double DoubleValue;

        [FieldOffset(0)]
        public jobject ObjectValue;
    }
}
