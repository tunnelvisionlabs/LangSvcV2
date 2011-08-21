namespace Tvl.Java.DebugInterface
{
    /// <summary>
    /// A mirror that has a <see cref="ILocation"/>.
    /// </summary>
    public interface ILocatable
    {
        /// <summary>
        /// Returns the <see cref="ILocation"/> of this mirror, if there is executable code associated with it.
        /// </summary>
        /// <returns>The <see cref="ILocation"/> of this mirror, or null if there is no executable code associated with it.</returns>
        ILocation GetLocation();
    }
}
