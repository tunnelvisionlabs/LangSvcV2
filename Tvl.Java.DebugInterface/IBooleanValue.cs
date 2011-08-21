namespace Tvl.Java.DebugInterface
{
    using System;

    /// <summary>
    /// Provides access to a primitive boolean value in the target VM.
    /// </summary>
    public interface IBooleanValue : IPrimitiveValue, IEquatable<IBooleanValue>
    {
        /// <summary>
        /// Returns this IBooleanValue as a bool.
        /// </summary>
        bool GetValue();
    }
}
