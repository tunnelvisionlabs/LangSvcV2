namespace Tvl.Java.DebugInterface.Request
{
    public interface IThreadStartRequest : IEventRequest
    {
        void AddThreadFilter(IThreadReference thread);
    }
}
