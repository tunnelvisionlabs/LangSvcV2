namespace Tvl.Java.DebugInterface.Request
{
    public interface IExceptionRequest : IEventRequest, IClassFilter, IInstanceFilter, IThreadFilter
    {
        IReferenceType GetException();

        bool GetNotifyCaught();

        bool GetNotifyUncaught();
    }
}
