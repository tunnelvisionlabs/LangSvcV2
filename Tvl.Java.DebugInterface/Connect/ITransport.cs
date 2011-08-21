namespace Tvl.Java.DebugInterface.Connect
{
    /// <summary>
    /// A method of communication between a debugger and a target VM.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Returns a short identifier for the transport.
        /// </summary>
        string GetName();
    }
}
