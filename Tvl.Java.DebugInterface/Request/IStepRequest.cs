namespace Tvl.Java.DebugInterface.Request
{
    public interface IStepRequest : IEventRequest, IClassFilter, IInstanceFilter
    {
        int GetDepth();

        StepSize GetSize();

        IThreadReference GetThread();
    }
}
