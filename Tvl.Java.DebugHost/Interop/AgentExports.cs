namespace Tvl.Java.DebugHost.Interop
{
    using System.Runtime.InteropServices;
    using IntPtr = System.IntPtr;

    public static class AgentExports
    {
        public static unsafe int OnLoad(IntPtr vmPtr, IntPtr optionsPtr, IntPtr reserved)
        {
            JavaVM vm = (JavaVM)Marshal.PtrToStructure(vmPtr, typeof(JavaVM));

            string options = null;
            if (optionsPtr != IntPtr.Zero)
                options = ModifiedUTF8Encoding.GetString((byte*)optionsPtr);

            JvmEnvironment env = vm.GetEnvironment(jvmtiVersion.Current);

            return 0;
        }

        public static unsafe int OnAttach(IntPtr vmPtr, IntPtr optionsPtr, IntPtr reserved)
        {
            JavaVM vm = (JavaVM)Marshal.PtrToStructure(vmPtr, typeof(JavaVM));

            string options = null;
            if (optionsPtr != IntPtr.Zero)
                options = ModifiedUTF8Encoding.GetString((byte*)optionsPtr);

            return 0;
        }

        public static void OnUnload(IntPtr vmPtr)
        {
        }
    }
}
