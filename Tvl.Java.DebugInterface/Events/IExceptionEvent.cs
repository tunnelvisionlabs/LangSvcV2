namespace Tvl.Java.DebugInterface.Events
{
    public interface IExceptionEvent : ILocatableEvent
    {
        ILocation GetCatchLocation();

        IObjectReference GetException();
    }
}
