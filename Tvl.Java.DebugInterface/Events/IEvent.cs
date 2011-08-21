namespace Tvl.Java.DebugInterface.Events
{
    using Tvl.Java.DebugInterface.Request;

    public interface IEvent : IMirror
    {
        IEventRequest GetRequest();
    }
}
