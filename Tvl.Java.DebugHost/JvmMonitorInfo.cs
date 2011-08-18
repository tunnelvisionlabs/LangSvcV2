namespace Tvl.Java.DebugHost
{
    using System;
    using System.Diagnostics.Contracts;

    public class JvmMonitorInfo : IDisposable
    {
        private readonly JvmObjectReference _monitor;
        private readonly int _stackDepth;

        public JvmMonitorInfo(JvmObjectReference monitor, int stackDepth)
        {
            Contract.Requires<ArgumentNullException>(monitor != null, "monitor");

            _monitor = monitor;
            _stackDepth = stackDepth;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            JvmObjectReference monitor = _monitor;
            if (monitor != null)
                monitor.Dispose();
        }
    }
}
