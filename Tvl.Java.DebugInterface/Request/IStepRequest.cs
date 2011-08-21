namespace Tvl.Java.DebugInterface.Request
{
    public interface IStepRequest : IEventRequest
    {
        void AddClassExclusionFilter(string classPattern);

        void AddClassFilter(IReferenceType referenceType);

        void AddClassFilter(string classPattern);

        void AddInstanceFilter(IObjectReference instance);

        int GetDepth();

        StepSize GetSize();

        IThreadReference GetThread();
    }
}
