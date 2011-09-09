namespace Tvl.Java.DebugHost.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;

    public class JniException : Exception
    {
        private readonly JniEnvironment _nativeEnvironment;
        private readonly jthrowable _throwable;

        public JniException(JniEnvironment nativeEnvironment, jthrowable throwable)
        {
            Contract.Requires<ArgumentNullException>(nativeEnvironment != null, "nativeEnvironment");
        }
    }
}
