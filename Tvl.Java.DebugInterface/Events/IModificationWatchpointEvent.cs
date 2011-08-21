namespace Tvl.Java.DebugInterface.Events
{
    public interface IModificationWatchpointEvent : IWatchpointEvent
    {
        IValue GetNewValue();
    }
}
