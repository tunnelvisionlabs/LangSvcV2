namespace Tvl.Java.DebugInterface.Types
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public struct ClassId : IEquatable<ClassId>
    {
        [DataMember(IsRequired = true)]
        public long Handle;

        public ClassId(long handle)
        {
            Handle = handle;
        }

        public static implicit operator ReferenceTypeId(ClassId @class)
        {
            return new ReferenceTypeId(@class.Handle);
        }

        public static explicit operator ClassId(ReferenceTypeId referenceType)
        {
            return new ClassId(referenceType.Handle);
        }

        public static bool operator ==(ClassId x, ClassId y)
        {
            return x.Handle == y.Handle;
        }

        public static bool operator !=(ClassId x, ClassId y)
        {
            return x.Handle != y.Handle;
        }

        public bool Equals(ClassId other)
        {
            return this.Handle == other.Handle;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ClassId))
                return false;

            return this.Handle == ((ClassId)obj).Handle;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }
    }
}
