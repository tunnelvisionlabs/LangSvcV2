namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;
    using AccessModifiers = Tvl.Java.DebugInterface.AccessModifiers;
    using Tvl.Java.DebugInterface.Types;
    using System.Collections.ObjectModel;

    internal abstract class ReferenceType : JavaType, IReferenceType
    {
        private readonly TaggedReferenceTypeId _taggedTypeId;

        protected ReferenceType(VirtualMachine virtualMachine, TaggedReferenceTypeId taggedTypeId)
            : base(virtualMachine)
        {
            Contract.Requires(virtualMachine != null);
            _taggedTypeId = taggedTypeId;
        }

        public ReferenceTypeId ReferenceTypeId
        {
            get
            {
                return _taggedTypeId.TypeId;
            }
        }

        public TaggedReferenceTypeId TaggedReferenceTypeId
        {
            get
            {
                return _taggedTypeId;
            }
        }

        public sealed override string GetSignature()
        {
            string signature;
            string genericSignature;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetSignature(out signature, out genericSignature, ReferenceTypeId));
            return signature;
        }

        #region IReferenceType Members

        public ReadOnlyCollection<IField> GetFields(bool includeInherited)
        {
            DeclaredFieldData[] fieldData;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetFields(out fieldData, ReferenceTypeId));
            Field[] fields = Array.ConvertAll(fieldData, field => VirtualMachine.GetMirrorOf(this, field));
            return new ReadOnlyCollection<IField>(fields);
        }

        public ReadOnlyCollection<ILocation> GetLineLocations()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<ILocation> GetLineLocations(string stratum, string sourceName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IMethod> GetMethods(bool includeInherited)
        {
            DeclaredMethodData[] methodData;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetMethods(out methodData, ReferenceTypeId));
            Method[] methods = Array.ConvertAll(methodData, method => VirtualMachine.GetMirrorOf(this, method));
            return new ReadOnlyCollection<IMethod>(methods);
        }

        public ReadOnlyCollection<string> GetAvailableStrata()
        {
            throw new NotImplementedException();
        }

        public IClassLoaderReference GetClassLoader()
        {
            ClassLoaderId classLoader;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetClassLoader(out classLoader, ReferenceTypeId));
            return VirtualMachine.GetMirrorOf(classLoader);
        }

        public IClassObjectReference GetClassObject()
        {
            ClassObjectId classObject;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetClassObject(out classObject, ReferenceTypeId));
            return VirtualMachine.GetMirrorOf(classObject);
        }

        public byte[] GetConstantPool()
        {
            throw new NotImplementedException();
        }

        public int GetConstantPoolCount()
        {
            throw new NotImplementedException();
        }

        public string GetDefaultStratum()
        {
            return "Java";
        }

        public bool GetFailedToInitialize()
        {
            ClassStatus status;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetReferenceTypeStatus(out status, ReferenceTypeId));
            return (status & ClassStatus.Error) != 0;
        }

        public IField GetFieldByName(string fieldName)
        {
            throw new NotImplementedException();
        }

        public string GetGenericSignature()
        {
            string signature;
            string genericSignature;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetSignature(out signature, out genericSignature, ReferenceTypeId));
            return genericSignature;
        }

        public IValue GetValue(IField field)
        {
            throw new NotImplementedException();
        }

        public IDictionary<IField, IValue> GetValues(IEnumerable<IField> fields)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IObjectReference> GetInstances(long maxInstances)
        {
            throw new NotImplementedException();
        }

        public bool GetIsAbstract()
        {
            return (GetModifiers() & AccessModifiers.Abstract) != 0;
        }

        public bool GetIsFinal()
        {
            return (GetModifiers() & AccessModifiers.Final) != 0;
        }

        public bool GetIsInitialized()
        {
            ClassStatus status;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetReferenceTypeStatus(out status, ReferenceTypeId));
            return (status & ClassStatus.Initialized) != 0;
        }

        public bool GetIsPrepared()
        {
            ClassStatus status;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetReferenceTypeStatus(out status, ReferenceTypeId));
            return (status & ClassStatus.Prepared) != 0;
        }

        public bool GetIsStatic()
        {
            return (GetModifiers() & AccessModifiers.Static) != 0;
        }

        public bool GetIsVerified()
        {
            ClassStatus status;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetReferenceTypeStatus(out status, ReferenceTypeId));
            return (status & ClassStatus.Verified) != 0;
        }

        public ReadOnlyCollection<ILocation> GetLocationsOfLine(int lineNumber)
        {
            string stratum = GetDefaultStratum();
            ReadOnlyCollection<string> paths = GetSourcePaths(stratum);
            return paths.SelectMany(i => GetLocationsOfLine(stratum, i, lineNumber)).ToList().AsReadOnly();
        }

        public ReadOnlyCollection<ILocation> GetLocationsOfLine(string stratum, string sourceName, int lineNumber)
        {
            if (stratum != "Java")
                return new ReadOnlyCollection<ILocation>(new ILocation[0]);

            List<ILocation> locations = new List<ILocation>();
            foreach (var method in GetMethods(false))
            {
                if (method.GetIsNative())
                    continue;

                locations.AddRange(method.GetLocationsOfLine(stratum, sourceName, lineNumber));
            }

            return locations.AsReadOnly();
        }

        public int GetMajorVersion()
        {
            throw new NotImplementedException();
        }

        public int GetMinorVersion()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IMethod> GetMethodsByName(string name)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IMethod> GetMethodsByName(string name, string signature)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IReferenceType> GetNestedTypes()
        {
            throw new NotImplementedException();
        }

        public string GetSourceDebugExtension()
        {
            throw new NotImplementedException();
        }

        public string GetSourceName()
        {
            string sourceFile;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetSourceFile(out sourceFile, ReferenceTypeId));
            return sourceFile;
        }

        public ReadOnlyCollection<string> GetSourceNames(string stratum)
        {
            if (stratum != "Java")
                return new ReadOnlyCollection<string>(new string[0]);

            return new ReadOnlyCollection<string>(new[] { GetSourceName() });
        }

        public ReadOnlyCollection<string> GetSourcePaths(string stratum)
        {
            // TODO: get actual source paths
            return GetSourceNames(stratum);
        }

        public ReadOnlyCollection<IField> GetVisibleFields()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IMethod> GetVisibleMethods()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAccessible Members

        public bool GetIsPackagePrivate()
        {
            return (GetModifiers() & (AccessModifiers.Private | AccessModifiers.Protected | AccessModifiers.Public)) == 0;
        }

        public bool GetIsPrivate()
        {
            return (GetModifiers() & AccessModifiers.Private) != 0;
        }

        public bool GetIsProtected()
        {
            return (GetModifiers() & AccessModifiers.Protected) != 0;
        }

        public bool GetIsPublic()
        {
            return (GetModifiers() & AccessModifiers.Public) != 0;
        }

        public AccessModifiers GetModifiers()
        {
            Types.AccessModifiers modifiers;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetModifiers(out modifiers, ReferenceTypeId));
            return (AccessModifiers)modifiers;
        }

        #endregion

        #region IEquatable<IReferenceType> Members

        public bool Equals(IReferenceType other)
        {
            ReferenceType otherType = other as ReferenceType;
            if (otherType == null)
                return false;

            return this.VirtualMachine.Equals(otherType.VirtualMachine)
                && this.ReferenceTypeId == otherType.ReferenceTypeId;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ReferenceType);
        }

        public override int GetHashCode()
        {
            return VirtualMachine.GetHashCode() ^ ReferenceTypeId.GetHashCode();
        }

        #endregion
    }
}
