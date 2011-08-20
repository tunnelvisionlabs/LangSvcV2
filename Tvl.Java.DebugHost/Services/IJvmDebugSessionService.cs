namespace Tvl.Java.DebugHost.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;

    [ServiceContract]
    public interface IJvmDebugSessionService
    {
        [OperationContract]
        void Attach();

        [OperationContract]
        void Detach();

        [OperationContract]
        void Terminate();
    }
}
