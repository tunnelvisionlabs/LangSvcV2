namespace Tvl.Java.DebugInterface.Request
{
    public interface IThreadDeathRequest : IEventRequest
    {
        void AddThreadFilter(IThreadReference thread);
    }
}
