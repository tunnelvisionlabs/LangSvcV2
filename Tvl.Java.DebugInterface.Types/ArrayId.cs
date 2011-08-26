namespace Tvl.Java.DebugInterface.Types
{
    using System.Runtime.Serialization;

    [DataContract]
    public struct ArrayId
    {
        [DataMember(IsRequired = true)]
        public long Handle;

        public ArrayId(long handle)
        {
            Handle = handle;
        }

        public static implicit operator ObjectId(ArrayId array)
        {
            return new ObjectId(array.Handle);
        }

        public static explicit operator ArrayId(ObjectId @object)
        {
            return new ArrayId(@object.Handle);
        }
    }
}
