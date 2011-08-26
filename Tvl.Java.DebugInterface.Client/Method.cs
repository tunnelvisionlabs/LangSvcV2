namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MethodId = Tvl.Java.DebugInterface.Types.MethodId;
    using System.Diagnostics.Contracts;
    using System.Collections.ObjectModel;
    using System.Text.RegularExpressions;
    using Path = System.IO.Path;

    internal sealed class Method : TypeComponent, IMethod
    {
        private readonly MethodId _methodId;
        private readonly ReadOnlyCollection<string> _argumentTypeNames;
        private readonly string _returnTypeName;

        internal Method(VirtualMachine virtualMachine, ReferenceType declaringType, string name, string signature, string genericSignature, AccessModifiers modifiers, MethodId methodId)
            : base(virtualMachine, declaringType, name, signature, genericSignature, modifiers)
        {
            Contract.Requires(virtualMachine != null);
            _methodId = methodId;

            List<string> argumentTypeNames;
            string returnTypeName;
            SignatureHelper.ParseMethodSignature(signature, out argumentTypeNames, out returnTypeName);

            _argumentTypeNames = argumentTypeNames.AsReadOnly();
            _returnTypeName = returnTypeName;
        }

        public MethodId MethodId
        {
            get
            {
                return _methodId;
            }
        }

        public ReadOnlyCollection<string> ArgumentTypeNames
        {
            get
            {
                return _argumentTypeNames;
            }
        }

        public string ReturnTypeName
        {
            get
            {
                return _returnTypeName;
            }
        }

        #region IMethod Members

        public ReadOnlyCollection<ILocation> GetLineLocations()
        {
            long startCodeIndex;
            long endCodeIndex;
            Types.LineNumberData[] lines;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetMethodLineTable(out startCodeIndex, out endCodeIndex, out lines, DeclaringType.ReferenceTypeId, this.MethodId));

            List<ILocation> locations = new List<ILocation>();
            foreach (var line in lines)
            {
                locations.Add(VirtualMachine.GetMirrorOf(this, line));
            }

            return locations.AsReadOnly();
        }

        public ReadOnlyCollection<ILocation> GetLineLocations(string stratum, string sourceName)
        {
            if (stratum != "Java")
                return new ReadOnlyCollection<ILocation>(new ILocation[0]);

            throw new NotImplementedException();
        }

        public ReadOnlyCollection<ILocalVariable> GetArguments()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<string> GetArgumentTypeNames()
        {
            return _argumentTypeNames;
        }

        public ReadOnlyCollection<IType> GetArgumentTypes()
        {
            throw new NotImplementedException();
        }

        public byte[] GetBytecodes()
        {
            byte[] result;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetMethodBytecodes(out result, DeclaringType.ReferenceTypeId, MethodId));
            return result;
        }

        public bool GetIsAbstract()
        {
            return (GetModifiers() & AccessModifiers.Abstract) != 0;
        }

        public bool GetIsBridge()
        {
            return (GetModifiers() & AccessModifiers.Bridge) != 0;
        }

        public bool GetIsConstructor()
        {
            return Name == "<init>";
        }

        public bool GetIsNative()
        {
            return (GetModifiers() & AccessModifiers.Native) != 0;
        }

        public bool GetIsObsolete()
        {
            bool result;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetMethodIsObsolete(out result, DeclaringType.ReferenceTypeId, MethodId));
            return result;
        }

        public bool GetIsStaticInitializer()
        {
            return Name == "<clinit>";
        }

        public bool GetIsSynchronized()
        {
            return (GetModifiers() & AccessModifiers.Synchronized) != 0;
        }

        public bool GetIsVarArgs()
        {
            return (GetModifiers() & AccessModifiers.VarArgs) != 0;
        }

        public ILocation GetLocationOfCodeIndex(long codeIndex)
        {
            return new Location(VirtualMachine, this, codeIndex);
        }

        public ReadOnlyCollection<ILocation> GetLocationsOfLine(int lineNumber)
        {
            return GetLocationsOfLine(DeclaringType.GetDefaultStratum(), DeclaringType.GetSourceName(), lineNumber);
        }

        public ReadOnlyCollection<ILocation> GetLocationsOfLine(string stratum, string sourceName, int lineNumber)
        {
            if (stratum != "Java")
                return new ReadOnlyCollection<ILocation>(new ILocation[0]);

            if (Path.GetFileName(sourceName) != Path.GetFileName(DeclaringType.GetSourceName()))
                return new ReadOnlyCollection<ILocation>(new ILocation[0]);

            long startCodeIndex;
            long endCodeIndex;
            Types.LineNumberData[] lines;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetMethodLineTable(out startCodeIndex, out endCodeIndex, out lines, DeclaringType.ReferenceTypeId, this.MethodId));

            List<ILocation> locations = new List<ILocation>();
            foreach (var line in lines)
            {
                if (line.LineNumber != lineNumber)
                    continue;

                locations.Add(VirtualMachine.GetMirrorOf(this, line));
            }

            return locations.AsReadOnly();
        }

        public IType GetReturnType()
        {
            return VirtualMachine.FindType(GetReturnTypeName());
        }

        public string GetReturnTypeName()
        {
            return _returnTypeName;
        }

        public ReadOnlyCollection<ILocalVariable> GetVariables()
        {
            Types.VariableData[] slots;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetMethodVariableTable(out slots, DeclaringType.ReferenceTypeId, MethodId));
            LocalVariable[] variables = Array.ConvertAll(slots, i => VirtualMachine.GetMirrorOf(this, i));
            return new ReadOnlyCollection<ILocalVariable>(variables);
        }

        public ReadOnlyCollection<ILocalVariable> GetVariablesByName(string name)
        {
            Types.VariableData[] slots;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetMethodVariableTable(out slots, DeclaringType.ReferenceTypeId, MethodId));
            List<ILocalVariable> variables = new List<ILocalVariable>(slots.Where(i => i.Name == name).Select(i => VirtualMachine.GetMirrorOf(this, i)));
            return variables.AsReadOnly();
        }

        #endregion

        #region ILocatable Members

        public ILocation GetLocation()
        {
            long startCodeIndex;
            long endCodeIndex;
            Types.LineNumberData[] lines;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetMethodLineTable(out startCodeIndex, out endCodeIndex, out lines, DeclaringType.ReferenceTypeId, this.MethodId));
            return VirtualMachine.GetMirrorOf(this, lines[0]);
        }

        #endregion

        #region IEquatable<IMethod> Members

        public bool Equals(IMethod other)
        {
            Method method = other as Method;
            if (method == null)
                return false;

            return this.VirtualMachine.Equals(method.VirtualMachine)
                && this.MethodId == method.MethodId;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Method);
        }

        public override int GetHashCode()
        {
            return this.VirtualMachine.GetHashCode() ^ this.MethodId.GetHashCode();
        }

        #endregion
    }
}
