namespace Tvl.Java.DebugInterface.Request
{
    public interface IStepRequest : IEventRequest, IClassFilter, IInstanceFilter
    {
        StepDepth Depth
        {
            get;
        }

        StepSize Size
        {
            get;
        }

        IThreadReference Thread
        {
            get;
        }
    }
}
