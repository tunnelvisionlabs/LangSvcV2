namespace Tvl.Java.DebugInterface.Connect
{
    using System.Collections.Generic;

    /// <summary>
    /// A method of connection between a debugger and a target VM.
    /// </summary>
    /// <remarks>
    /// A method of connection between a debugger and a target VM. A connector encapsulates exactly one
    /// <see cref="ITransport"/> used to establish the connection. Each connector has a set of arguments
    /// which controls its operation. The arguments are stored as a dictionary, keyed by a string. Each
    /// implementation defines the string argument keys it accepts.
    /// </remarks>
    public interface IConnector
    {
        /// <summary>
        /// Returns the arguments accepted by this connector and their default values.
        /// </summary>
        IDictionary<string, IConnectorArgument> GetDefaultArguments();

        /// <summary>
        /// Returns a human-readable description of this connector and its purpose.
        /// </summary>
        string GetDescription();

        /// <summary>
        /// Returns a short identifier for the connector.
        /// </summary>
        string GetName();

        /// <summary>
        /// Returns the transport mechanism used by this connector to establish connections with a target VM.
        /// </summary>
        ITransport GetTransport();
    }
}
