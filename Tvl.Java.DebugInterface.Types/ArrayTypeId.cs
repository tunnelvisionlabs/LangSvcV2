namespace Tvl.Java.DebugInterface.Types
{
    using System.Runtime.Serialization;

    [DataContract]
    public struct ArrayTypeId
    {
        [DataMember(IsRequired = true)]
        public long Handle;

        public ArrayTypeId(long handle)
        {
            Handle = handle;
        }

        public static implicit operator ReferenceTypeId(ArrayTypeId @array)
        {
            return new ReferenceTypeId(@array.Handle);
        }

        public static explicit operator ArrayTypeId(ReferenceTypeId referenceType)
        {
            return new ArrayTypeId(referenceType.Handle);
        }
    }
}
