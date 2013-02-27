namespace Tvl.VisualStudio.Language.Parsing4
{
    using Antlr.Runtime;

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
