namespace Tvl.Java.DebugInterface.Events
{
    using System.Collections.Generic;
    using Tvl.Java.DebugInterface.Request;

    public interface IEventSet : IMirror, ICollection<IEvent>
    {
        void Resume();

        SuspendPolicy SuspendPolicy();
    }
}
