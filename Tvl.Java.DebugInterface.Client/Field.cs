namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugInterface.Types;
    using AccessModifiers = Tvl.Java.DebugInterface.AccessModifiers;

    internal sealed class Field : TypeComponent, IField
    {
        private readonly FieldId _fieldId;

        internal Field(VirtualMachine virtualMachine, ReferenceType declaringType, string name, string signature, string genericSignature, AccessModifiers modifiers, FieldId fieldId)
            : base(virtualMachine, declaringType, name, signature, genericSignature, modifiers)
        {
            Contract.Requires(virtualMachine != null);
            _fieldId = fieldId;
        }

        public FieldId FieldId
        {
            get
            {
                return _fieldId;
            }
        }

        #region IField Members

        public bool GetIsEnumConstant()
        {
            throw new NotImplementedException();
        }

        public bool GetIsTransient()
        {
            return (GetModifiers() & AccessModifiers.Transient) != 0;
        }

        public bool GetIsVolatile()
        {
            return (GetModifiers() & AccessModifiers.Volatile) != 0;
        }

        public IType GetFieldType()
        {
            return VirtualMachine.FindType(GetSignature());
        }

        public string GetFieldTypeName()
        {
            return GetFieldType().GetName();
        }

        #endregion

        #region IEquatable<IField> Members

        public bool Equals(IField other)
        {

            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
