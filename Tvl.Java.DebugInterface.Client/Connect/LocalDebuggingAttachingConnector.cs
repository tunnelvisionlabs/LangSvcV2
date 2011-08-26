namespace Tvl.Java.DebugInterface.Client.Connect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.Java.DebugInterface.Connect;

    public sealed class LocalDebuggingAttachingConnector : IAttachingConnector
    {
        private readonly ITransport _transport = new NetNamedPipeTransport();

        private readonly Dictionary<string, IConnectorArgument> _defaultArguments =
            new Dictionary<string, IConnectorArgument>();

        public LocalDebuggingAttachingConnector()
        {
            _defaultArguments.Add("pid", new ConnectorIntegerArgument("pid", "Process ID", "The system process ID of the process to attach to.", true, 0, 0, int.MaxValue));
        }

        #region IAttachingConnector Members

        public IVirtualMachine Attach(IEnumerable<KeyValuePair<string, IConnectorArgument>> arguments)
        {
            var pid = (IConnectorIntegerArgument)arguments.Single(i => i.Key == "pid").Value;

            VirtualMachine virtualMachine = VirtualMachine.BeginAttachToProcess(pid.Value);
            return virtualMachine;
        }

        #endregion

        #region IConnector Members

        public IDictionary<string, IConnectorArgument> DefaultArguments
        {
            get
            {
                return _defaultArguments;
            }
        }

        public string Description
        {
            get
            {
                return "Local debugging attaching connector";
            }
        }

        public string Name
        {
            get
            {
                return "Local debugging attaching connector";
            }
        }

        public ITransport Transport
        {
            get
            {
                return _transport;
            }
        }

        #endregion
    }
}
