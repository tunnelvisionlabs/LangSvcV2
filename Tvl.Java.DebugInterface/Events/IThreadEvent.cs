namespace Tvl.Java.DebugInterface.Events
{
    public interface IThreadEvent : IEvent
    {
        IThreadReference GetThread();
    }
}
