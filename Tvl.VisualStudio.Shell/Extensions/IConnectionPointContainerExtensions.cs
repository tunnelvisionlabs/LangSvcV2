namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using IConnectionPoint = Microsoft.VisualStudio.OLE.Interop.IConnectionPoint;
    using IConnectionPointContainer = Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer;

    public static class IConnectionPointContainerExtensions
    {
        [NotNull]
        public static IDisposable Advise<TObject, TEventInterface>([NotNull] this IConnectionPointContainer container, [NotNull] TObject @object)
            where TObject : class, TEventInterface
            where TEventInterface : class
        {
            Requires.NotNull(container, nameof(container));
            Requires.NotNull(@object, nameof(@object));

            Guid eventGuid = typeof(TEventInterface).GUID;
            IConnectionPoint connectionPoint;
            container.FindConnectionPoint(eventGuid, out connectionPoint);
            if (connectionPoint == null)
                throw new ArgumentException();

            uint cookie;
            connectionPoint.Advise(@object, out cookie);
            return new ConnectionPointCookie(connectionPoint, cookie);
        }

        public static void Unadvise<TEventInterface>([NotNull] this IConnectionPointContainer container, uint cookie)
            where TEventInterface : class
        {
            Requires.NotNull(container, nameof(container));

            if (cookie == 0)
                return;

            Guid eventGuid = typeof(TEventInterface).GUID;
            IConnectionPoint connectionPoint;
            container.FindConnectionPoint(eventGuid, out connectionPoint);
            if (connectionPoint == null)
                throw new ArgumentException();

            connectionPoint.Unadvise(cookie);
        }

        private sealed class ConnectionPointCookie : IDisposable
        {
            private readonly Tvl.WeakReference<IConnectionPoint> _connectionPoint;
            private uint _cookie;

            public ConnectionPointCookie([NotNull] IConnectionPoint connectionPoint, uint cookie)
            {
                Debug.Assert(connectionPoint != null);

                _connectionPoint = new Tvl.WeakReference<IConnectionPoint>(connectionPoint);
                _cookie = cookie;
            }

            public void Dispose()
            {
                if (_cookie != 0)
                {
                    IConnectionPoint connectionPoint = _connectionPoint.Target;
                    if (connectionPoint != null)
                    {
                        connectionPoint.Unadvise(_cookie);
                        _cookie = 0;
                    }
                }
            }
        }
    }
}
