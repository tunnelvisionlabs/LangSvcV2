namespace Tvl.Java.DebugInterface.Events
{
    public interface IMethodExitEvent : ILocatableEvent
    {
        IMethod GetMethod();

        IValue GetReturnValue();
    }
}
