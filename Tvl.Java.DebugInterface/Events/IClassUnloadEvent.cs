namespace Tvl.Java.DebugInterface.Events
{
    public interface IClassUnloadEvent : IEvent
    {
        string GetClassName();

        string GetClassSignature();
    }
}
