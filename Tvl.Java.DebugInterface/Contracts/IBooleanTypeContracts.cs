namespace Tvl.Java.DebugInterface.Contracts
{
    using System.Diagnostics.Contracts;
    using System;

    [ContractClassFor(typeof(IBooleanType))]
    internal abstract class IBooleanTypeContracts : IBooleanType
    {
        #region IType Members

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public string GetSignature()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMirror Members

        public IVirtualMachine GetVirtualMachine()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
