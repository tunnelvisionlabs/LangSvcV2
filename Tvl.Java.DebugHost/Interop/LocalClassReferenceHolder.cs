namespace Tvl.Java.DebugHost.Interop
{
    using System;

    public struct LocalClassReferenceHolder : IDisposable
    {
        private readonly JniEnvironment _nativeEnvironment;
        private readonly jclass _reference;

        public LocalClassReferenceHolder(JniEnvironment nativeEnvironment, jclass reference)
        {
            _nativeEnvironment = nativeEnvironment;
            _reference = reference;
            _reference = (jclass)_nativeEnvironment.NewLocalReference(reference);
        }

        public jclass Value
        {
            get
            {
                return _reference;
            }
        }

        public bool IsAlive
        {
            get
            {
                return !_nativeEnvironment.IsSameObject(Value, jclass.Null);
            }
        }

        public void Dispose()
        {
            if (IsAlive)
                _nativeEnvironment.DeleteLocalReference(_reference);
        }
    }
}
