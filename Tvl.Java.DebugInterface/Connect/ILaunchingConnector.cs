namespace Tvl.Java.DebugInterface.Connect
{
    using System.Collections.Generic;

    /// <summary>
    /// A connector which can launch a target VM before connecting to it.
    /// </summary>
    public interface ILaunchingConnector : IConnector
    {
        /// <summary>
        /// Launches an application and connects to its VM.
        /// </summary>
        IVirtualMachine Launch(IEnumerable<KeyValuePair<string, IConnectorArgument>> arguments);
    }
}
