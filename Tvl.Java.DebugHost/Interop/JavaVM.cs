// The field 'field_name' is never used
#pragma warning disable 169

namespace Tvl.Java.DebugHost.Interop
{
    using System.Runtime.InteropServices;
    using IntPtr = System.IntPtr;

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct JavaVM
    {
        private readonly IntPtr _jniInvokeInterface;

        public JvmEnvironment GetEnvironment(jvmtiVersion version)
        {
            JniInvokeInterface jniInvokeInterface = (JniInvokeInterface)Marshal.PtrToStructure(_jniInvokeInterface, typeof(JniInvokeInterface));

            jvmtiEnvHandle env;
            jniInvokeInterface.GetEnv(this, out env, version);

            return JvmEnvironment.GetOrCreateEnvironment(env);
        }
    }
}
