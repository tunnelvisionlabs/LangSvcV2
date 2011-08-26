namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;

    internal static class JvmtiErrorHandler
    {
        public static void ThrowOnFailure(JvmToolsService.jvmtiError error)
        {
            if (error != JvmToolsService.jvmtiError.None)
                throw new Exception(error.ToString());
        }
    }
}
