namespace Tvl.Extensions
{
    using System;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;

    public static class ReaderWriterLockSlimExtensions
    {
        public static ReadLockHelper ReadLock(this ReaderWriterLockSlim readerWriterLock)
        {
            if (readerWriterLock == null)
                throw new ArgumentNullException("readerWriterLock");

            return new ReadLockHelper(readerWriterLock);
        }

        public static UpgradeableReadLockHelper UpgradableReadLock(this ReaderWriterLockSlim readerWriterLock)
        {
            if (readerWriterLock == null)
                throw new ArgumentNullException("readerWriterLock");

            return new UpgradeableReadLockHelper(readerWriterLock);
        }

        public static WriteLockHelper WriteLock(this ReaderWriterLockSlim readerWriterLock)
        {
            if (readerWriterLock == null)
                throw new ArgumentNullException("readerWriterLock");

            return new WriteLockHelper(readerWriterLock);
        }

        public struct ReadLockHelper : IDisposable
        {
            private readonly ReaderWriterLockSlim _readerWriterLock;

            public ReadLockHelper(ReaderWriterLockSlim readerWriterLock)
            {
                readerWriterLock.EnterReadLock();
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

            public UpgradeableReadLockHelper(ReaderWriterLockSlim readerWriterLock)
            {
                this._readerWriterLock = readerWriterLock;
                this._readonly = false;
                this._readerWriterLock.EnterUpgradeableReadLock();
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

            public WriteLockHelper(ReaderWriterLockSlim readerWriterLock)
            {
                readerWriterLock.EnterWriteLock();
                this._readerWriterLock = readerWriterLock;
            }

            public void Dispose()
            {
                this._readerWriterLock.ExitWriteLock();
            }
        }
    }
}
