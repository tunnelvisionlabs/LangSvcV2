namespace Tvl.Java.DebugInterface.Client
{
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugInterface.Types;

    internal sealed class ClassObjectReference : ObjectReference, IClassObjectReference
    {
        internal ClassObjectReference(VirtualMachine virtualMachine, ClassObjectId objectId)
            : base(virtualMachine, objectId)
        {
            Contract.Requires(virtualMachine != null);
        }

        public ClassObjectId ClassObjectId
        {
            get
            {
                return (ClassObjectId)base.ObjectId;
            }
        }

        internal override Types.Value ToNetworkValue()
        {
            return new Types.Value(Tag.ClassObject, ObjectId.Handle);
        }

        public IReferenceType GetReflectedType()
        {
            TypeTag typeTag;
            ReferenceTypeId typeId;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetReflectedType(out typeTag, out typeId, this.ClassObjectId));
            return VirtualMachine.GetMirrorOf(typeTag, typeId);
        }
    }
}
