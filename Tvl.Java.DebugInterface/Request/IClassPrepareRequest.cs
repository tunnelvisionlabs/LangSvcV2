namespace Tvl.Java.DebugInterface.Request
{
    public interface IClassPrepareRequest : IEventRequest, IClassFilter
    {
        void AddSourceNameFilter(string sourceNamePattern);
    }
}
