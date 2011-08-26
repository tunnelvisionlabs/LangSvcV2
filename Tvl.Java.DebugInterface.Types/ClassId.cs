namespace Tvl.Java.DebugInterface.Types
{
    using System.Runtime.Serialization;

    [DataContract]
    public struct ClassId
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
    }
}
