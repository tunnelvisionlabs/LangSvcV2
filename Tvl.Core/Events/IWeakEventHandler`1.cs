namespace Tvl.Core.Events
{
    using System;

    public interface IWeakEventHandler<TEventArgs>
        where TEventArgs : EventArgs
    {
        EventHandler<TEventArgs> Handler
        {
            get;
        }
    }
}
