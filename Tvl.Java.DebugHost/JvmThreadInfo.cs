namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.Java.DebugHost.Interop;

    public sealed class JvmThreadInfo
    {
        public readonly string Name;

        public readonly int Priority;

        public readonly bool IsDaemon;

        public readonly JvmThreadGroupReference ThreadGroup;

        public readonly JvmObjectReference ContextClassLoader;

        internal JvmThreadInfo(JvmEnvironment environment, JNIEnvHandle jniEnv, jvmtiThreadInfo threadInfo, bool freeLocalReference)
        {
            this.Name = threadInfo.Name;
            this.Priority = threadInfo._priority;
            this.IsDaemon = threadInfo._isDaemon != 0;
            this.ThreadGroup = JvmThreadGroupReference.FromHandle(environment, jniEnv, threadInfo._threadGroup, freeLocalReference);
            this.ContextClassLoader = JvmObjectReference.FromHandle(environment, jniEnv, threadInfo._contextClassLoader, freeLocalReference);
        }
    }
}
