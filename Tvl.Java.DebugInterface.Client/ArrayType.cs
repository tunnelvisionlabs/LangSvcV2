namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.Java.DebugInterface.Types;
    using System.Diagnostics.Contracts;

    internal sealed class ArrayType : ReferenceType, IArrayType
    {
        internal ArrayType(VirtualMachine virtualMachine, ArrayTypeId typeId)
            : base(virtualMachine, new TaggedReferenceTypeId(TypeTag.Array, typeId))
        {
            Contract.Requires(virtualMachine != null);
        }

        public string GetComponentSignature()
        {
            return GetSignature().Substring(1);
        }

        public IType GetComponentType()
        {
            return VirtualMachine.FindType(GetComponentSignature());
        }

        public string GetComponentTypeName()
        {
            return SignatureHelper.DecodeTypeName(GetSignature().Substring(1));
        }

        public IArrayReference CreateInstance(int length)
        {
            throw new NotImplementedException();
        }
    }
}
