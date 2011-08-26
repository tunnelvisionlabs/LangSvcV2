namespace Tvl.Java.DebugInterface.Types
{
    using System.Runtime.Serialization;
    using System;

    [DataContract]
    public struct TaggedObjectId : IEquatable<TaggedObjectId>
    {
        [DataMember(IsRequired = true)]
        public Tag Tag;

        [DataMember(IsRequired = true)]
        public ObjectId ObjectId;

        public TaggedObjectId(Tag tag, ObjectId objectId)
        {
            Tag = tag;
            ObjectId = objectId;
        }

        public static bool operator ==(TaggedObjectId x, TaggedObjectId y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(TaggedObjectId x, TaggedObjectId y)
        {
            return !x.Equals(y);
        }

        public bool Equals(TaggedObjectId other)
        {
            return this.Tag == other.Tag
                && this.ObjectId == other.ObjectId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TaggedObjectId))
                return false;

            return this.Equals((TaggedObjectId)obj);
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode() ^ ObjectId.GetHashCode();
        }
    }
}
