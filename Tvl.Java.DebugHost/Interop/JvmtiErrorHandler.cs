namespace Tvl.Java.DebugHost.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class JvmtiErrorHandler
    {
        public static void ThrowOnFailure(jvmtiError error)
        {
            if (error == jvmtiError.None)
                return;

            throw new Exception(string.Format("{0} ({1})", error, (int)error));
        }
    }
}
