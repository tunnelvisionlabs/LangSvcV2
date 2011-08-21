namespace Tvl.Java.DebugInterface.Events
{
    public interface IWatchpointEvent : ILocatableEvent
    {
        IField GetField();

        IObjectReference GetObject();

        IValue GetCurrentValue();
    }
}
