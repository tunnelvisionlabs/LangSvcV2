namespace Tvl.Java.DebugHost.Services
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public struct JvmVirtualMachineRemoteHandle
    {
        [DataMember(IsRequired = true)]
        public long Handle;

        public JvmVirtualMachineRemoteHandle(IntPtr handle)
        {
            Handle = handle.ToInt64();
        }
    }
}
