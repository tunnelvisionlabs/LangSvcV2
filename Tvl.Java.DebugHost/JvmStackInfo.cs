namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.Java.DebugHost.Interop;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;

    public class JvmStackInfo : IDisposable
    {
        private readonly JvmThreadReference _thread;
        private readonly jvmtiThreadState _state;
        private readonly ReadOnlyCollection<JvmLocation> _frames;

        internal JvmStackInfo(JvmThreadReference thread, jvmtiThreadState state, ReadOnlyCollection<JvmLocation> frames)
        {
            Contract.Requires<ArgumentNullException>(thread != null, "thread");
            Contract.Requires<ArgumentNullException>(frames != null, "frames");

            _thread = thread;
            _state = state;
            _frames = frames;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            JvmThreadReference thread = _thread;
            if (thread != null)
                thread.Dispose();
        }
    }
}
