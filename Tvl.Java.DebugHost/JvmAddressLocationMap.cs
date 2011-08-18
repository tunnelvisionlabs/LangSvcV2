namespace Tvl.Java.DebugHost
{
    using IntPtr = System.IntPtr;

    public class JvmAddressLocationMap
    {
        public readonly JvmLocation Location;
        public readonly IntPtr StartAddress;

        internal JvmAddressLocationMap(JvmLocation location, IntPtr startAddress)
        {
            Location = location;
            StartAddress = startAddress;
        }
    }
}
