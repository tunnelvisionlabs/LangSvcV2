namespace Tvl.Events
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    public static class WeakEvents
    {
        public static EventHandler<TEventArgs> MakeWeak<TEventArgs>(EventHandler<TEventArgs> handler, Action<EventHandler<TEventArgs>> unregister)
            where TEventArgs : EventArgs
        {
            Contract.Requires<ArgumentNullException>(handler != null);

            if (handler.Method.IsStatic)
                return handler;

            Type t = typeof(WeakEventHandler<,>).MakeGenericType(handler.Method.DeclaringType, typeof(TEventArgs));
            ConstructorInfo ctor = t.GetConstructor(new Type[] { typeof(EventHandler<TEventArgs>), typeof(Action<EventHandler<TEventArgs>>) });
            IWeakEventHandler<TEventArgs> weakHandler = (IWeakEventHandler<TEventArgs>)ctor.Invoke(new object[] { });
            return weakHandler.Handler;
        }
    }
}
