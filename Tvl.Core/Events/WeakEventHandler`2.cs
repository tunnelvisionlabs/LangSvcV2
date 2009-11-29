namespace Tvl.Core.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class WeakEventHandler<T, E>
        where T : class
        where E : EventArgs
    {
        private delegate void OpenEventHandler(T @this, object sender, E e);

        private WeakReference _target;
        private OpenEventHandler _openHandler;
        private EventHandler<E> _handler;
        private Action<EventHandler<E>> _unregister;

        public WeakEventHandler(EventHandler<E> handler, Action<EventHandler<E>> unregister)
        {
            _target = new WeakReference(handler.Target);
            _openHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, handler.Method);
            _handler = Invoke;
            _unregister = unregister;
        }

        public void Invoke(object sender, E e)
        {
            T target = (T)_target.Target;

            if (target != null)
            {
                _openHandler.Invoke(target, sender, e);
            }
            else if (this._unregister != null)
            {
                _unregister(_handler);
                _unregister = null;
            }
        }
    }
}
