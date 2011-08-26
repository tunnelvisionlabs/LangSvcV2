namespace Tvl.Java.DebugInterface.Request
{
    public interface IExceptionRequest : IEventRequest, IClassFilter, IInstanceFilter, IThreadFilter
    {
        IReferenceType Exception
        {
            get;
        }

        bool NotifyWhenCaught
        {
            get;
        }

        bool NotifyWhenUncaught
        {
            get;
        }
    }
}
