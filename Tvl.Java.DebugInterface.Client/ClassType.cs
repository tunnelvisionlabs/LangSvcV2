namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using InvokeOptions = Tvl.Java.DebugInterface.InvokeOptions;
    using Tvl.Java.DebugInterface.Types;
    using System.Diagnostics.Contracts;
    using AccessModifiers = Tvl.Java.DebugInterface.AccessModifiers;
    using System.Collections.ObjectModel;

    internal sealed class ClassType : ReferenceType, IClassType
    {
        internal ClassType(VirtualMachine virtualMachine, ClassId typeId)
            : base(virtualMachine, new TaggedReferenceTypeId(TypeTag.Class, typeId))
        {
            Contract.Requires(virtualMachine != null);
        }

        public ReadOnlyCollection<IInterfaceType> GetInterfaces(bool includeInherited)
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
            return (GetModifiers() & AccessModifiers.Enum) != 0;
        }

        public IObjectReference CreateInstance(IThreadReference thread, IMethod method, InvokeOptions options, params IValue[] arguments)
        {
            throw new NotImplementedException();
        }

        public void SetValue(IField field, IValue value)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IClassType> GetSubclasses()
        {
            throw new NotImplementedException();
        }

        public IClassType GetSuperclass()
        {
            throw new NotImplementedException();
        }
    }
}
