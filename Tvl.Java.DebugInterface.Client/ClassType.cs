namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using InvokeOptions = Tvl.Java.DebugInterface.InvokeOptions;
    using Tvl.Java.DebugInterface.Types;
    using System.Diagnostics.Contracts;

    internal sealed class ClassType : ReferenceType, IClassType
    {
        internal ClassType(VirtualMachine virtualMachine, ClassId typeId)
            : base(virtualMachine, typeId)
        {
            Contract.Requires(virtualMachine != null);
        }

        public IList<IInterfaceType> GetInterfaces(bool includeInherited)
        {
            throw new NotImplementedException();
        }

        public IMethod GetConcreteMethod(string name, string signature)
        {
            throw new NotImplementedException();
        }

        public IValue InvokeMethod(IThreadReference thread, IMethod method, InvokeOptions options, params IValue[] arguments)
        {
            throw new NotImplementedException();
        }

        public bool GetIsEnum()
        {
            throw new NotImplementedException();
        }

        public IObjectReference CreateInstance(IThreadReference thread, IMethod method, InvokeOptions options, params IValue[] arguments)
        {
            throw new NotImplementedException();
        }

        public void SetValue(IField field, IValue value)
        {
            throw new NotImplementedException();
        }

        public IList<IClassType> GetSubclasses()
        {
            throw new NotImplementedException();
        }

        public IClassType GetSuperclass()
        {
            throw new NotImplementedException();
        }
    }
}
