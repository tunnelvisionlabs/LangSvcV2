namespace Tvl.Java.DebugHost.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;

    [ServiceContract(CallbackContract = typeof(IJvmEvents), SessionMode = SessionMode.Required)]
    public interface IJvmEventsService
    {
        [OperationContract]
        void Subscribe(JvmEventType eventType);

        [OperationContract]
        void Unsubscribe(JvmEventType eventType);
    }
}
