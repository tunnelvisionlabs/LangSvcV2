namespace Tvl.Java.DebugInterface
{
    using System;

    /// <summary>
    /// Provides access to a primitive short value in the target VM.
    /// </summary>
    public interface IShortValue : IPrimitiveValue, IComparable<IShortValue>, IEquatable<IShortValue>
    {
        /// <summary>
        /// Returns this IShortValue as a short.
        /// </summary>
        short GetValue();
    }
}
