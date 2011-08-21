namespace Tvl.Java.DebugInterface.Events
{
    public interface IMethodEntryEvent : ILocatableEvent
    {
        IMethod GetMethod();
    }
}
