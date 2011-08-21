namespace Tvl.Java.DebugInterface
{
    using System;

    /// <summary>
    /// Provides access to a primitive long value in the target VM.
    /// </summary>
    public interface ILongValue : IPrimitiveValue, IComparable<ILongValue>, IEquatable<ILongValue>
    {
        /// <summary>
        /// Returns this ILongValue as a long.
        /// </summary>
        long GetValue();
    }
}
