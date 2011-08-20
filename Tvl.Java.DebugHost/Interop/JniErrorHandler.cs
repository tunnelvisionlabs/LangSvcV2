namespace Tvl.Java.DebugHost.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class JniErrorHandler
    {
        public static void ThrowOnFailure(int result)
        {
            if (result != 0)
                throw new Exception("JNI Exception Occurred.");
        }
    }
}
