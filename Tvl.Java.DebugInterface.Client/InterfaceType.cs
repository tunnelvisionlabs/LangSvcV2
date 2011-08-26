namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.Java.DebugInterface.Types;
    using System.Diagnostics.Contracts;
    using System.Collections.ObjectModel;

    internal sealed class InterfaceType : ReferenceType, IInterfaceType
    {
        internal InterfaceType(VirtualMachine virtualMachine, InterfaceId typeId)
            : base(virtualMachine, typeId)
        {
            Contract.Requires(virtualMachine != null);
        }

        public ReadOnlyCollection<IClassType> GetImplementors()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IInterfaceType> GetSubInterfaces()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IInterfaceType> GetSuperInterfaces()
        {
            throw new NotImplementedException();
        }
    }
}
