namespace Tvl.Java.DebugInterface.Types
{
    using System.Runtime.Serialization;

    [DataContract]
    public struct StringId
    {
        [DataMember(IsRequired = true)]
        public long Handle;

        public StringId(long handle)
        {
            Handle = handle;
        }

        public static implicit operator ObjectId(StringId classObject)
        {
            return new ObjectId(classObject.Handle);
        }

        public static explicit operator StringId(ObjectId @object)
        {
            return new StringId(@object.Handle);
        }
    }
}
