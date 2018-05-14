namespace Tvl
{
    using System;
    using JetBrains.Annotations;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;

    public static class ReaderWriterLockSlimExtensions
    {
        public static ReadLockHelper ReadLock([NotNull] this ReaderWriterLockSlim readerWriterLock)
        {
            Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

            return new ReadLockHelper(readerWriterLock);
        }

        public static ReadLockHelper ReadLock([NotNull] this ReaderWriterLockSlim readerWriterLock, int millisecondsTimeout)
        {
            Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

            return new ReadLockHelper(readerWriterLock, millisecondsTimeout);
        }

        public static ReadLockHelper ReadLock([NotNull] this ReaderWriterLockSlim readerWriterLock, TimeSpan timeout)
        {
            Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

            return new ReadLockHelper(readerWriterLock, timeout);
        }

        public static UpgradeableReadLockHelper UpgradableReadLock([NotNull] this ReaderWriterLockSlim readerWriterLock)
        {
            Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

            return new UpgradeableReadLockHelper(readerWriterLock);
        }

        public static UpgradeableReadLockHelper UpgradableReadLock([NotNull] this ReaderWriterLockSlim readerWriterLock, int millisecondsTimeout)
        {
            Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

            return new UpgradeableReadLockHelper(readerWriterLock, millisecondsTimeout);
        }

        public static UpgradeableReadLockHelper UpgradableReadLock([NotNull] this ReaderWriterLockSlim readerWriterLock, TimeSpan timeout)
        {
            Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

            return new UpgradeableReadLockHelper(readerWriterLock, timeout);
        }

        public static WriteLockHelper WriteLock([NotNull] this ReaderWriterLockSlim readerWriterLock)
        {
            Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

            return new WriteLockHelper(readerWriterLock);
        }

        public static WriteLockHelper WriteLock([NotNull] this ReaderWriterLockSlim readerWriterLock, int millisecondsTimeout)
        {
            Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

            return new WriteLockHelper(readerWriterLock, millisecondsTimeout);
        }

        public static WriteLockHelper WriteLock([NotNull] this ReaderWriterLockSlim readerWriterLock, TimeSpan timeout)
        {
            Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

            return new WriteLockHelper(readerWriterLock, timeout);
        }

        public struct ReadLockHelper : IDisposable
        {
            private readonly ReaderWriterLockSlim _readerWriterLock;

            public ReadLockHelper([NotNull] ReaderWriterLockSlim readerWriterLock)
            {
                Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

                readerWriterLock.EnterReadLock();
                this._readerWriterLock = readerWriterLock;
            }

            public ReadLockHelper([NotNull] ReaderWriterLockSlim readerWriterLock, int millisecondsTimeout)
            {
                Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

                if (!readerWriterLock.TryEnterReadLock(millisecondsTimeout))
                    throw new TimeoutException();

                this._readerWriterLock = readerWriterLock;
            }

            public ReadLockHelper([NotNull] ReaderWriterLockSlim readerWriterLock, TimeSpan timeout)
            {
                Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

                if (!readerWriterLock.TryEnterReadLock(timeout))
                    throw new TimeoutException();

                this._readerWriterLock = readerWriterLock;
            }

            public void Dispose()
            {
                this._readerWriterLock.ExitReadLock();
            }
        }

        public struct UpgradeableReadLockHelper : IDisposable
        {
            private readonly ReaderWriterLockSlim _readerWriterLock;
            private bool _readonly;

            public UpgradeableReadLockHelper([NotNull] ReaderWriterLockSlim readerWriterLock)
            {
                Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

                this._readerWriterLock = readerWriterLock;
                this._readonly = false;
                this._readerWriterLock.EnterUpgradeableReadLock();
            }

            public UpgradeableReadLockHelper([NotNull] ReaderWriterLockSlim readerWriterLock, int millisecondsTimeout)
            {
                Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

                this._readerWriterLock = readerWriterLock;
                this._readonly = false;
                if (!this._readerWriterLock.TryEnterUpgradeableReadLock(millisecondsTimeout))
                    throw new TimeoutException();
            }

            public UpgradeableReadLockHelper([NotNull] ReaderWriterLockSlim readerWriterLock, TimeSpan timeout)
            {
                Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

                this._readerWriterLock = readerWriterLock;
                this._readonly = false;
                if (!this._readerWriterLock.TryEnterUpgradeableReadLock(timeout))
                    throw new TimeoutException();
            }

            public bool IsReadOnly
            {
                get
                {
                    return _readonly;
                }
            }

            public void Dispose()
            {
                if (IsReadOnly)
                    this._readerWriterLock.ExitReadLock();
                else
                    this._readerWriterLock.ExitUpgradeableReadLock();
            }

            public void Downgrade()
            {
                this._readerWriterLock.EnterReadLock();
                this._readerWriterLock.ExitUpgradeableReadLock();
                _readonly = true;
            }

            public WriteLockHelper WriteLock()
            {
                return new WriteLockHelper(this._readerWriterLock);
            }
        }

        public struct WriteLockHelper : IDisposable
        {
            private readonly ReaderWriterLockSlim _readerWriterLock;

            public WriteLockHelper([NotNull] ReaderWriterLockSlim readerWriterLock)
            {
                Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

                readerWriterLock.EnterWriteLock();
                this._readerWriterLock = readerWriterLock;
            }

            public WriteLockHelper([NotNull] ReaderWriterLockSlim readerWriterLock, int millisecondsTimeout)
            {
                Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

                if (!readerWriterLock.TryEnterWriteLock(millisecondsTimeout))
                    throw new TimeoutException();

                this._readerWriterLock = readerWriterLock;
            }

            public WriteLockHelper([NotNull] ReaderWriterLockSlim readerWriterLock, TimeSpan timeout)
            {
                Requires.NotNull(readerWriterLock, nameof(readerWriterLock));

                if (!readerWriterLock.TryEnterWriteLock(timeout))
                    throw new TimeoutException();

                this._readerWriterLock = readerWriterLock;
            }

            public void Dispose()
            {
                this._readerWriterLock.ExitWriteLock();
            }
        }
    }
}
