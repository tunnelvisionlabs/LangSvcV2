namespace Tvl.Java.DebugInterface.Connect
{
    using System.Collections.ObjectModel;

    public interface IConnectorSelectedArgument : IConnectorArgument
    {
        ReadOnlyCollection<string> GetChoices();
    }
}
