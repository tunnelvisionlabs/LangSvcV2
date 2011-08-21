namespace Tvl.Java.DebugInterface.Connect
{
    /// <summary>
    /// Specification for and value of a <see cref="IConnector"/> argument.
    /// </summary>
    public interface IConnectorArgument
    {
        /// <summary>
        /// Returns a human-readable description of this argument and its purpose.
        /// </summary>
        string GetDescription();

        /// <summary>
        /// Performs basic sanity check of argument.
        /// </summary>
        bool GetIsValid(string value);

        /// <summary>
        /// Returns a short human-readable label for this argument.
        /// </summary>
        string GetLabel();

        /// <summary>
        /// Indicates whether the argument must be specified.
        /// </summary>
        bool GetMustSpecify();

        /// <summary>
        /// Returns a short, unique identifier for the argument.
        /// </summary>
        string GetName();

        /// <summary>
        /// Sets the value of the argument.
        /// </summary>
        void SetValue(string value);

        /// <summary>
        /// Returns the current value of the argument.
        /// </summary>
        string GetValue();
    }
}
