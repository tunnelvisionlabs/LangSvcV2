namespace Tvl.Java.DebugInterface.Request
{
    public interface IMonitorWaitRequest : IEventRequest
    {
        void AddClassExclusionFilter(string classPattern);

        void AddClassFilter(IReferenceType referenceType);

        void AddClassFilter(string classPattern);

        void AddInstanceFilter(IObjectReference instance);

        void AddThreadFilter(IThreadReference thread);
    }
}
