namespace Tvl.Java.DebugInterface.Client
{
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugInterface.Types;

    internal class StringReference : ObjectReference, IStringReference
    {
        internal StringReference(VirtualMachine virtualMachine, StringId stringId)
            : base(virtualMachine, stringId)
        {
            Contract.Requires(virtualMachine != null);
        }

        public StringId StringId
        {
            get
            {
                return (StringId)base.ObjectId;
            }
        }

        internal override Types.Value ToNetworkValue()
        {
            return new Types.Value(Tag.String, ObjectId.Handle);
        }

        public string GetValue()
        {
            string value;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetStringValue(out value, StringId));
            return value;
        }
    }
}
