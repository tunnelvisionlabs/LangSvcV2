namespace Tvl.Java.DebugInterface.Types
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class ConstantMethodReference : ConstantPoolEntry
    {
        [DataMember]
        private readonly ushort _classIndex;
        [DataMember]
        private readonly ushort _nameAndTypeIndex;

        public ConstantMethodReference(ushort classIndex, ushort nameAndTypeIndex)
        {
            _classIndex = classIndex;
            _nameAndTypeIndex = nameAndTypeIndex;
        }

        public override ConstantType Type
        {
            get
            {
                return ConstantType.MethodReference;
            }
        }

        public ushort ClassIndex
        {
            get
            {
                return _classIndex;
            }
        }

        public ushort NameAndTypeIndex
        {
            get
            {
                return _nameAndTypeIndex;
            }
        }

        public override string ToString(ReadOnlyCollection<ConstantPoolEntry> constantPool)
        {
            ConstantPoolEntry classEntry = constantPool[ClassIndex - 1];
            ConstantPoolEntry nameAndTypeEntry = constantPool[NameAndTypeIndex - 1];
            return classEntry.ToString(constantPool) + "." + nameAndTypeEntry.ToString(constantPool);
        }
    }
}
