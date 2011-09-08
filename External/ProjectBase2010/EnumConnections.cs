namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Tvl.Collections;
    using CONNECTDATA = Microsoft.VisualStudio.OLE.Interop.CONNECTDATA;
    using IEnumConnections = Microsoft.VisualStudio.OLE.Interop.IEnumConnections;

    [ComVisible(true)]
    public class EnumConnections<SinkType> : IEnumConnections
        where SinkType : class
    {
        private readonly ImmutableList<KeyValuePair<uint, SinkType>> _connections;
        private int _currentIndex;

        public EnumConnections(IEnumerable<KeyValuePair<uint, SinkType>> connections)
        {
            Contract.Requires<ArgumentNullException>(connections != null, "connections");

            _connections = new ImmutableList<KeyValuePair<uint, SinkType>>(connections);
        }

        private EnumConnections(ImmutableList<KeyValuePair<uint, SinkType>> connections, int currentIndex)
        {
            Contract.Requires<ArgumentNullException>(connections != null, "connections");

            _connections = connections;
            _currentIndex = currentIndex;
        }

        #region IEnumConnections Members

        public void Clone(out IEnumConnections ppEnum)
        {
            ppEnum = new EnumConnections<SinkType>(_connections, _currentIndex);
        }

        public int Next(uint cConnections, CONNECTDATA[] rgcd, out uint pcFetched)
        {
            pcFetched = 0;

            if (rgcd == null || rgcd.Length < cConnections)
                return VSConstants.E_INVALIDARG;

            int remaining = _connections.Count - _currentIndex;
            pcFetched = checked((uint)Math.Min(cConnections, remaining));
            for (int i = 0; i < pcFetched; i++)
            {
                rgcd[i].dwCookie = _connections[_currentIndex + i].Key;
                rgcd[i].punk = _connections[_currentIndex + i].Value;
            }

            _currentIndex += (int)pcFetched;
            return pcFetched == cConnections ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        public int Reset()
        {
            _currentIndex = 0;
            return VSConstants.S_OK;
        }

        public int Skip(uint cConnections)
        {
            int remaining = _connections.Count - _currentIndex;
            if (remaining < cConnections)
            {
                _currentIndex = _connections.Count;
                return VSConstants.S_FALSE;
            }

            _currentIndex += (int)cConnections;
            return VSConstants.S_OK;
        }

        #endregion
    }
}
