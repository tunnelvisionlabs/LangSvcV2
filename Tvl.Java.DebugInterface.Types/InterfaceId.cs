namespace Tvl.Java.DebugInterface.Types
{
    using System.Runtime.Serialization;
    using System;

    [DataContract]
    public struct InterfaceId
    {
        [DataMember(IsRequired = true)]
        public long Handle;

        public InterfaceId(long handle)
        {
            Handle = handle;
        }

        public static implicit operator ReferenceTypeId(InterfaceId @interface)
        {
            return new ReferenceTypeId(@interface.Handle);
        }

        public static explicit operator InterfaceId(ReferenceTypeId referenceType)
        {
            return new InterfaceId(referenceType.Handle);
        }

        public static explicit operator InterfaceId(TaggedReferenceTypeId referenceType)
        {
            if (referenceType.TypeTag != TypeTag.Interface)
                throw new ArgumentException();

            return new InterfaceId(referenceType.TypeId.Handle);
        }
    }
}
