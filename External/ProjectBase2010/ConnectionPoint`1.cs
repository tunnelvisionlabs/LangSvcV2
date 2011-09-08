/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;

    public class ConnectionPoint<SinkType> : IConnectionPoint
        where SinkType : class
    {
        private readonly Dictionary<uint, SinkType> sinks;
        private readonly ConnectionPointContainer container;
        private readonly IEventSource<SinkType> source;
        private uint nextCookie;

        internal ConnectionPoint(ConnectionPointContainer container, IEventSource<SinkType> source)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(source != null, "source");

            this.sinks = new Dictionary<uint, SinkType>();
            this.container = container;
            this.source = source;
            this.nextCookie = 1;
        }

        #region IConnectionPoint Members
        public void Advise(object pUnkSink, out uint pdwCookie)
        {
            SinkType sink = pUnkSink as SinkType;
            if (sink == null)
                Marshal.ThrowExceptionForHR(VSConstants.E_NOINTERFACE);

            sinks.Add(nextCookie, sink);
            pdwCookie = nextCookie;
            source.OnSinkAdded(sink);
            nextCookie += 1;
        }

        public void EnumConnections(out IEnumConnections ppEnum)
        {
            ppEnum = new EnumConnections<SinkType>(sinks);
        }

        public void GetConnectionInterface(out Guid pIID)
        {
            pIID = typeof(SinkType).GUID;
        }

        public void GetConnectionPointContainer(out IConnectionPointContainer ppCPC)
        {
            ppCPC = this.container;
        }

        public void Unadvise(uint dwCookie)
        {
            // This will throw if the cookie is not in the list.
            SinkType sink = sinks[dwCookie];
            sinks.Remove(dwCookie);
            source.OnSinkRemoved(sink);
        }
        #endregion
    }
}
