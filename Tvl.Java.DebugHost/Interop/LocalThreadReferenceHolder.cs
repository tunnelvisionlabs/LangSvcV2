namespace Tvl.Java.DebugHost.Interop
{
    using System;

    public struct LocalThreadReferenceHolder : IDisposable
    {
        private readonly JniEnvironment _nativeEnvironment;
        private readonly jthread _reference;

        public LocalThreadReferenceHolder(JniEnvironment nativeEnvironment, jthread reference)
        {
            _nativeEnvironment = nativeEnvironment;
            _reference = reference;
            _reference = (jthread)_nativeEnvironment.NewLocalReference(reference);
        }

        public jthread Value
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
                if (_nativeEnvironment == null)
                    return false;

                return !_nativeEnvironment.IsSameObject(Value, jthread.Null);
            }
        }

        public void Dispose()
        {
            if (IsAlive)
                _nativeEnvironment.DeleteLocalReference(_reference);
        }
    }
}
