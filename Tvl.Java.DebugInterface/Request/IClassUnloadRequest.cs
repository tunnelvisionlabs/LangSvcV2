namespace Tvl.Java.DebugInterface.Request
{
    public interface IClassUnloadRequest : IEventRequest
    {
        void AddClassExclusionFilter(string classPattern);

        void AddClassFilter(string classPattern);
    }
}
