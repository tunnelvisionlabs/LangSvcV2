namespace Tvl.Java.DebugInterface.Events
{
    public interface IClassPrepareEvent : IEvent
    {
        IReferenceType GetReferenceType();

        IThreadReference GetThread();
    }
}
