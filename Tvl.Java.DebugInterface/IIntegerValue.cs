namespace Tvl.Java.DebugInterface
{
    using System;

    /// <summary>
    /// Provides access to a primitive int value in the target VM.
    /// </summary>
    public interface IIntegerValue : IPrimitiveValue, IComparable<IIntegerValue>, IEquatable<IIntegerValue>
    {
        /// <summary>
        /// Returns this IIntegerValue as an int.
        /// </summary>
        int GetValue();
    }
}
