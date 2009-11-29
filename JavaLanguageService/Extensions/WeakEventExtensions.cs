namespace JavaLanguageService.Extensions
{
    using System;
    using JavaLanguageService.ShellServices;

    public static class WeakEventExtensions
    {
        public static EventHandler<TEventArgs> AsWeak<TEventArgs>(this EventHandler<TEventArgs> handler, Action<EventHandler<TEventArgs>> unregister)
            where TEventArgs : EventArgs
        {
            return WeakEvents.MakeWeak(handler, unregister);
        }
    }
}
