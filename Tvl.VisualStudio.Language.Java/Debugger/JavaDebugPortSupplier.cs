namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;

    [ComVisible(true)]
    public class JavaDebugPortSupplier : IDebugPortSupplier3, IDebugPortSupplier2, IDebugPortSupplierDescription2, IDebugPortSupplierEx2, IDebugPortSupplierLocale2
    {
        #region IDebugPortSupplier2 Members

        public int AddPort(IDebugPortRequest2 pRequest, out IDebugPort2 ppPort)
        {
            throw new NotImplementedException();
        }

        public int CanAddPort()
        {
            throw new NotImplementedException();
        }

        public int EnumPorts(out IEnumDebugPorts2 ppEnum)
        {
            throw new NotImplementedException();
        }

        public int GetPort(ref Guid guidPort, out IDebugPort2 ppPort)
        {
            throw new NotImplementedException();
        }

        public int GetPortSupplierId(out Guid pguidPortSupplier)
        {
            throw new NotImplementedException();
        }

        public int GetPortSupplierName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int RemovePort(IDebugPort2 pPort)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugPortSupplier3 Members

        public int CanPersistPorts()
        {
            throw new NotImplementedException();
        }

        public int EnumPersistedPorts(BSTR_ARRAY PortNames, out IEnumDebugPorts2 ppEnum)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugPortSupplierDescription2 Members

        public int GetDescription(enum_PORT_SUPPLIER_DESCRIPTION_FLAGS[] pdwFlags, out string pbstrText)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugPortSupplierEx2 Members

        public int SetServer(IDebugCoreServer2 pServer)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDebugPortSupplierLocale2 Members

        public int SetLocale(ushort wLangID)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
