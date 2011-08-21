namespace Tvl.Java.DebugInterface
{
    using System;

    /// <summary>
    /// Provides access to a primitive char value in the target VM.
    /// </summary>
    public interface ICharValue : IPrimitiveValue, IComparable<ICharValue>, IEquatable<ICharValue>
    {
        /// <summary>
        /// Returns this ICharValue as a char.
        /// </summary>
        char GetValue();
    }
}
