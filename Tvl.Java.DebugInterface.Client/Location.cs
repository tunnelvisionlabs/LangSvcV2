namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;

    internal sealed class Location : Mirror, ILocation
    {
        private readonly Method _method;
        private readonly long _codeIndex;

        internal Location(VirtualMachine virtualMachine, Method method, long codeIndex)
            : base(virtualMachine)
        {
            Contract.Requires(virtualMachine != null);
            Contract.Requires<ArgumentNullException>(method != null, "method");

            _method = method;
            _codeIndex = codeIndex;
        }

        public long CodeIndex
        {
            get
            {
                return _codeIndex;
            }
        }

        public Method Method
        {
            get
            {
                return _method;
            }
        }

        public long GetCodeIndex()
        {
            return _codeIndex;
        }

        public IReferenceType GetDeclaringType()
        {
            return _method.DeclaringType;
        }

        public int GetLineNumber()
        {
            long start;
            long end;
            Types.LineNumberData[] lines;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetMethodLineTable(out start, out end, out lines, _method.DeclaringType.ReferenceTypeId, _method.MethodId));

            if (_codeIndex < start || _codeIndex > end)
                throw new NativeMethodException();

            Types.LineNumberData lineData = lines.Last(i => i.LineCodeIndex <= _codeIndex);
            return lineData.LineNumber;
        }

        public int GetLineNumber(string stratum)
        {
            throw new NotImplementedException();
        }

        public IMethod GetMethod()
        {
            return _method;
        }

        public string GetSourceName()
        {
            string sourceFile;
            DebugErrorHandler.ThrowOnFailure(VirtualMachine.ProtocolService.GetSourceFile(out sourceFile, _method.DeclaringType.ReferenceTypeId));
            return sourceFile;
        }

        public string GetSourceName(string stratum)
        {
            throw new NotImplementedException();
        }

        public string GetSourcePath()
        {
            return GetSourceName();
            //GetMethod().GetDeclaringType().GetS
            //VirtualMachine.ProtocolService.gets
            //throw new NotImplementedException();
        }

        public string GetSourcePath(string stratum)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ILocation other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ILocation other)
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
    }
}
