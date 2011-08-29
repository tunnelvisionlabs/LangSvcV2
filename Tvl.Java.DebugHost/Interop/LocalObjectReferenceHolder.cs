namespace Tvl.Java.DebugHost.Interop
{
    using System;

    public struct LocalObjectReferenceHolder : IDisposable
    {
        private readonly JniEnvironment _nativeEnvironment;
        private readonly jobject _reference;

        public LocalObjectReferenceHolder(JniEnvironment nativeEnvironment, jobject reference)
        {
            _nativeEnvironment = nativeEnvironment;
            _reference = reference;
            _reference = _nativeEnvironment.NewLocalReference(reference);
        }

        public jobject Value
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
