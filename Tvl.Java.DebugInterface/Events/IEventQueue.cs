namespace Tvl.Java.DebugInterface.Events
{
    using System;

    public interface IEventQueue : IMirror
    {
        IEventSet Remove();

        IEventSet Remove(TimeSpan timeout);
    }
}
