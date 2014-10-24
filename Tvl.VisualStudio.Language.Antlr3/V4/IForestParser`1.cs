namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System.Collections.Generic;
    using Antlr4.Runtime;

    public interface IForestParser<T>
    {
        IDictionary<RuleContext, CaretReachedException> GetParseTrees(T parser);
    }
}
