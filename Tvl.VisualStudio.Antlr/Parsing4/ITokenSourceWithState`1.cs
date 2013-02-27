namespace Tvl.VisualStudio.Language.Parsing4
{
    using Antlr4.Runtime;

    public interface ITokenSourceWithState<T> : ITokenSource
        where T : struct
    {
        ICharStream CharStream
        {
            get;
        }

        T GetCurrentState();
    }
}
