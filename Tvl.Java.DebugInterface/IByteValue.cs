namespace Tvl.Java.DebugInterface
{
    using System;

    /// <summary>
    /// Provides access to a primitive byte value in the target VM.
    /// </summary>
    public interface IByteValue : IPrimitiveValue, IEquatable<IByteValue>, IComparable<IByteValue>
    {
        /// <summary>
        /// Returns this IByteValue as a byte.
        /// </summary>
        byte GetValue();
    }
}
