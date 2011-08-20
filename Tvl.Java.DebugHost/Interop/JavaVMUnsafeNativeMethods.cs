namespace Tvl.Java.DebugHost.Interop
{
    using System;
    using System.Runtime.InteropServices;

    public static class JavaVMUnsafeNativeMethods
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int DestroyJavaVM(JavaVM vm);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int AttachCurrentThread(JavaVM vm, out JNIEnvHandle penv, ref JavaVMAttachArgs args);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int DetachCurrentThread(JavaVM vm);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetEnv(JavaVM vm, out jvmtiEnvHandle penv, jvmtiVersion version);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int AttachCurrentThreadAsDaemon(JavaVM vm, out JNIEnvHandle penv, ref JavaVMAttachArgs args);
    }
}
