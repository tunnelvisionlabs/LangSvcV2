namespace Tvl.Java.DebugInterface.Client
{
    using System.Diagnostics.Contracts;

    internal abstract class Value : Mirror, IValue
    {
        protected Value(VirtualMachine virtualMachine)
            : base(virtualMachine)
        {
            Contract.Requires(virtualMachine != null);
        }

        public abstract IType GetValueType();

        internal abstract Types.Value ToNetworkValue();
    }
}
