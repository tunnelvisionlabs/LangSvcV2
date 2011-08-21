namespace Tvl.Java.DebugInterface.Connect
{
    public interface IConnectorIntegerArgument : IConnectorArgument
    {
        int GetIntValue();

        bool GetIsValid(int value);

        int GetMax();

        int GetMin();

        void SetValue(int value);

        string GetStringValueOf(int value);
    }
}
