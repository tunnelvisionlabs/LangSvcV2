namespace Tvl.Java.DebugInterface.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;
    using Tvl.Java.DebugInterface.Types;
    using System.Collections.ObjectModel;

    internal class ThreadGroupReference : ObjectReference, IThreadGroupReference
    {
        internal ThreadGroupReference(VirtualMachine virtualMachine, ThreadGroupId threadGroupId)
            : base(virtualMachine, threadGroupId)
        {
            Contract.Requires(virtualMachine != null);
        }

        public ThreadGroupId ThreadGroupId
        {
            get
            {
                return (ThreadGroupId)base.ObjectId;
            }
        }

        internal override Types.Value ToNetworkValue()
        {
            return new Types.Value(Tag.ThreadGroup, ObjectId.Handle);
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public IThreadGroupReference GetParent()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IThreadGroupReference> GetThreadGroups()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IThreadReference> GetThreads()
        {
            throw new NotImplementedException();
        }
    }
}
