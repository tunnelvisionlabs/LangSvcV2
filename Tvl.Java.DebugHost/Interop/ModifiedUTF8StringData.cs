namespace Tvl.Java.DebugHost.Interop
{
    using IntPtr = System.IntPtr;

    public struct ModifiedUTF8StringData
    {
        public readonly IntPtr _data;

        public string GetString()
        {
            unsafe
            {
                return ModifiedUTF8Encoding.GetString((byte*)_data);
            }
        }
    }
}
