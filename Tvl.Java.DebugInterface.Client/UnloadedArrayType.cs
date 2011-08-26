namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;

    internal sealed class UnloadedArrayType : UnloadedReferenceType, IArrayType
    {
        private readonly string _componentSignature;
        private readonly string _componentTypeName;
        private readonly UnloadedReferenceType _componentType;

        internal UnloadedArrayType(VirtualMachine virtualMachine, string signature)
            : base(virtualMachine, signature)
        {
            Contract.Requires(virtualMachine != null);
            Contract.Requires(!string.IsNullOrEmpty(signature));

            _componentSignature = Signature.Substring(1);
            _componentTypeName = SignatureHelper.DecodeTypeName(_componentSignature);
            _componentType = new UnloadedReferenceType(VirtualMachine, _componentSignature);
        }

        #region IArrayType Members

        public string GetComponentSignature()
        {
            return _componentSignature;
        }

        public IType GetComponentType()
        {
            return _componentType;
        }

        public string GetComponentTypeName()
        {
            return _componentTypeName;
        }

        public IArrayReference CreateInstance(int length)
        {
            throw new ClassNotLoadedException(GetName());
        }

        #endregion
    }
}
