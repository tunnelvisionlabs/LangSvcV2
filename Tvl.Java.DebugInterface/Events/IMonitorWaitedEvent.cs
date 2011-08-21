namespace Tvl.Java.DebugInterface.Events
{
    public interface IMonitorWaitedEvent : IMonitorEvent
    {
        bool GetTimedOut();
    }
}
