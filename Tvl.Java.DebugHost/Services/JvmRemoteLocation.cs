namespace Tvl.Java.DebugHost.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    [DataContract]
    public struct JvmRemoteLocation
    {
        [DataMember]
        public long Method;

        [DataMember]
        public long Location;
    }
}
