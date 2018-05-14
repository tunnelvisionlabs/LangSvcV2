namespace Tvl.Events
{
    using System;
    using System.Reflection;
    using JetBrains.Annotations;

    public static class WeakEvents
    {
        [NotNull]
        public static EventHandler AsWeak([NotNull] EventHandler handler, Action<EventHandler> unregister)
        {
            Requires.NotNull(handler, nameof(handler));

            if (handler.Method.IsStatic)
                return handler;

            Type t = typeof(WeakEventHandler<>).MakeGenericType(handler.Method.DeclaringType);
            ConstructorInfo ctor = t.GetConstructor(new Type[] { typeof(EventHandler), typeof(Action<EventHandler>) });
            IWeakEventHandler weakHandler = (IWeakEventHandler)ctor.Invoke(new object[] { handler, unregister });
            return weakHandler.Handler;
        }

        [NotNull]
        public static EventHandler<TEventArgs> AsWeak<TEventArgs>([NotNull] this EventHandler<TEventArgs> handler, Action<EventHandler<TEventArgs>> unregister)
            where TEventArgs : EventArgs
        {
            Requires.NotNull(handler, nameof(handler));

            if (handler.Method.IsStatic)
                return handler;

            Type t = typeof(WeakEventHandler<,>).MakeGenericType(handler.Method.DeclaringType, typeof(TEventArgs));
            ConstructorInfo ctor = t.GetConstructor(new Type[] { typeof(EventHandler<TEventArgs>), typeof(Action<EventHandler<TEventArgs>>) });
            IWeakEventHandler<TEventArgs> weakHandler = (IWeakEventHandler<TEventArgs>)ctor.Invoke(new object[] { handler, unregister });
            return weakHandler.Handler;
        }
    }
}
