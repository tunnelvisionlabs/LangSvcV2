namespace Tvl.Java.DebugInterface.Types
{
    using System.Runtime.Serialization;

    [DataContract]
    public struct ClassObjectId
    {
        [DataMember(IsRequired = true)]
        public long Handle;

        public ClassObjectId(long handle)
        {
            Handle = handle;
        }

        public static implicit operator ObjectId(ClassObjectId classObject)
        {
            return new ObjectId(classObject.Handle);
        }

        public static explicit operator ClassObjectId(ObjectId @object)
        {
            return new ClassObjectId(@object.Handle);
        }
    }
}
