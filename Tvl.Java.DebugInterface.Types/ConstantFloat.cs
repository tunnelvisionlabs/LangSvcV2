namespace Tvl.Java.DebugInterface.Types
{
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;

    [DataContract]
    public class ConstantFloat : ConstantPoolEntry
    {
        [DataMember]
        private readonly float _value;

        public ConstantFloat(float value)
        {
            _value = value;
        }

        public override ConstantType Type
        {
            get
            {
                return ConstantType.Float;
            }
        }

        public float Value
        {
            get
            {
                return _value;
            }
        }

        public override string ToString(ReadOnlyCollection<ConstantPoolEntry> constantPool)
        {
            return Value.ToString();
        }
    }
}
