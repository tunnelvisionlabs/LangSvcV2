namespace Tvl.Java.DebugInterface.Request
{
    public interface IClassPrepareRequest : IEventRequest
    {
        void AddClassExclusionFilter(string classPattern);

        void AddClassFilter(IReferenceType referenceType);

        void AddClassFilter(string classPattern);

        void AddSourceNameFilter(string sourceNamePattern);
    }
}
