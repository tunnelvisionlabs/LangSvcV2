namespace Tvl.VisualStudio.Language.AntlrV4
{
    using Antlr4.Runtime;

    public interface ICaretToken : IToken
    {
        IToken OriginalToken
        {
            get;
        }
    }
}
