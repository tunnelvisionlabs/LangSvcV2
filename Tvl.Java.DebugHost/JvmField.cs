namespace Tvl.Java.DebugHost
{
    using Tvl.Java.DebugHost.Interop;

    public class JvmField
    {
        private readonly JvmEnvironment _environment;
        private readonly jfieldID _fieldId;

        internal JvmField(JvmEnvironment environment, jfieldID fieldId)
        {
            _environment = environment;
            _fieldId = fieldId;
        }
    }
}
