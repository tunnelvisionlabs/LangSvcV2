namespace Tvl.Java.DebugInterface.Request
{
    public interface IWatchpointRequest : IEventRequest, IClassFilter, IInstanceFilter, IThreadFilter
    {
        IField Field
        {
            get;
        }
    }
}
