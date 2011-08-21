namespace Tvl.Java.DebugInterface.Events
{
    public interface IMonitorEvent : ILocatableEvent
    {
        IObjectReference GetMonitor();
    }
}
