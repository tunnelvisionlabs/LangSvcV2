namespace Tvl.Java.DebugInterface.Request
{
    public interface IBreakpointRequest : IEventRequest, ILocatable, IInstanceFilter, IThreadFilter
    {
    }
}
