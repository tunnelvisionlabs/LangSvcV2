namespace Tvl.Java.DebugInterface.Request
{
    public interface IBreakpointRequest : IEventRequest, ILocatable
    {
        void AddInstanceFilter(IObjectReference instance);

        void AddThreadFilter(IThreadReference thread);
    }
}
