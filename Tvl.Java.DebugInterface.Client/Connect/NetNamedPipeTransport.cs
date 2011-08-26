namespace Tvl.Java.DebugInterface.Client.Connect
{
    using Tvl.Java.DebugInterface.Connect;

    internal sealed class NetNamedPipeTransport : ITransport
    {
        public string GetName()
        {
            return "WCF Named Pipe Transport";
        }
    }
}
