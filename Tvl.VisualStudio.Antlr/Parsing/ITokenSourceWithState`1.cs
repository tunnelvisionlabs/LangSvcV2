namespace Tvl.VisualStudio.Language.Parsing
{
    using Antlr.Runtime;

    public interface ITokenSourceWithState<T> : ITokenSource
    {
        T GetCurrentState();
    }
}
