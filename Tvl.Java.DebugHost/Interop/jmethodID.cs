// Field 'field_name' is never assigned to, and will always have its default value null
#pragma warning disable 649

namespace Tvl.Java.DebugHost.Interop
{
    using System;

    internal struct jmethodID : IEquatable<jmethodID>
    {
        public static readonly jmethodID Null = default(jmethodID);

        private readonly IntPtr _handle;

        internal IntPtr Handle
        {
            get
            {
                return _handle;
            }
        }

        public static bool operator ==(jmethodID x, jmethodID y)
        {
            return x._handle == y._handle;
        }

        public static bool operator !=(jmethodID x, jmethodID y)
        {
            return x._handle != y._handle;
        }

        public static explicit operator jmethodID(JvmMethod method)
        {
            return method.MethodId;
        }

        public bool Equals(jmethodID other)
        {
            return this._handle == other._handle;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is jmethodID))
                return false;

            return this._handle == ((jmethodID)obj)._handle;
        }

        public override int GetHashCode()
        {
            return _handle.GetHashCode();
        }
    }
}
