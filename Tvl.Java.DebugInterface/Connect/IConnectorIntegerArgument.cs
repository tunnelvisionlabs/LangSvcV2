namespace Tvl.Java.DebugInterface.Connect
{
    public interface IConnectorIntegerArgument : IConnectorArgument
    {
        /// <summary>
        /// Gets or sets the value of the argument as an integer.
        /// </summary>
        int Value
        {
            get;
            set;
        }

        int MinimumValue
        {
            get;
        }

        int MaximumValue
        {
            get;
        }

        bool IsValid(int value);

        string GetStringValueOf(int value);
    }
}
