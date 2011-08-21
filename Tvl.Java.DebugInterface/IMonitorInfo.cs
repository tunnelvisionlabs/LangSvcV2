namespace Tvl.Java.DebugInterface
{
    /// <summary>
    /// Information about a monitor owned by a thread.
    /// </summary>
    public interface IMonitorInfo : IMirror
    {
        /// <summary>
        /// Returns the <see cref="IObjectReference"/> object for the monitor.
        /// </summary>
        IObjectReference GetMonitor();

        /// <summary>
        /// Returns the stack depth at which this monitor was acquired by the owning thread.
        /// </summary>
        int GetStackDepth();

        /// <summary>
        /// Returns a <see cref="IThreadReference"/> object for the thread that owns the monitor.
        /// </summary>
        IThreadReference GetThread();
    }
}
