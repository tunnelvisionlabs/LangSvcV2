namespace Tvl.Java.DebugInterface.Types
{
    using System.Runtime.Serialization;
    using System;

    [DataContract]
    public struct ThreadGroupId
    {
        [DataMember(IsRequired = true)]
        public long Handle;

        public ThreadGroupId(long handle)
        {
            Handle = handle;
        }

        public static implicit operator ObjectId(ThreadGroupId threadGroup)
        {
            return new ObjectId(threadGroup.Handle);
        }

        public static explicit operator ThreadGroupId(ObjectId @object)
        {
            return new ThreadGroupId(@object.Handle);
        }

        public static explicit operator ThreadGroupId(TaggedObjectId @object)
        {
            if (@object.Tag != Tag.ThreadGroup)
                throw new ArgumentException();

            return new ThreadGroupId(@object.ObjectId.Handle);
        }
    }
}
