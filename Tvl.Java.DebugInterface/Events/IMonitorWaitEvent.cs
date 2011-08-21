namespace Tvl.Java.DebugInterface.Events
{
    using TimeSpan = System.TimeSpan;

    public interface IMonitorWaitEvent : IMonitorEvent
    {
        TimeSpan GetTimeout();
    }
}
