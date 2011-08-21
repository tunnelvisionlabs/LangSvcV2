namespace Tvl.Java.DebugInterface.Request
{
    public interface IEventRequest : IMirror
    {
        void AddCountFilter(int count);

        void Disable();

        void Enable();

        object GetProperty(object key);

        bool GetIsEnabled();

        void PutProperty(object key, object value);

        void SetIsEnabled(bool value);

        SuspendPolicy GetSuspendPolicy();

        void SetSuspendPolicy(SuspendPolicy policy);
    }
}
