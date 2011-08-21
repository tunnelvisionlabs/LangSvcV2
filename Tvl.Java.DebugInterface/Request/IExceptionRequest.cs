namespace Tvl.Java.DebugInterface.Request
{
    public interface IExceptionRequest : IEventRequest
    {
        void AddClassExclusionFilter(string classPattern);

        void AddClassFilter(IReferenceType referenceType);

        void AddClassFilter(string classPattern);

        void AddInstanceFilter(IObjectReference instance);

        void AddThreadFilter(IThreadReference thread);

        IReferenceType GetException();

        bool GetNotifyCaught();

        bool GetNotifyUncaught();
    }
}
