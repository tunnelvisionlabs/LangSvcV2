namespace Tvl.Java.DebugInterface
{
    using System;

    /// <summary>
    /// Provides access to a primitive double value in the target VM.
    /// </summary>
    public interface IDoubleValue : IPrimitiveValue, IComparable<IDoubleValue>, IEquatable<IDoubleValue>
    {
        /// <summary>
        /// Returns this IDoubleValue as a double.
        /// </summary>
        double GetValue();
    }
}
