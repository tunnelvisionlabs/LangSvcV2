namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class JvmThreadGroupInfo
    {
        public readonly JvmThreadGroupReference Parent;

        public readonly string Name;

        public readonly int MaxPriority;

        public readonly bool IsDaemon;
    }
}
