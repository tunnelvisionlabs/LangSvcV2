namespace Tvl.Java.DebugInterface.Client.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.Java.DebugInterface.Client.Request;
    using Tvl.Java.DebugInterface.Request;

    public class ThreadEventArgs : VirtualMachineEventArgs
    {
        private readonly ThreadReference _thread;

        internal ThreadEventArgs(VirtualMachine virtualMachine, SuspendPolicy suspendPolicy, EventRequest request, ThreadReference thread)
            : base(virtualMachine, suspendPolicy, request)
        {
            _thread = thread;
        }

        public IThreadReference Thread
        {
            get
            {
                return _thread;
            }
        }
    }
}
