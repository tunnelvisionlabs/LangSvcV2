namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

    internal interface IElementReference<T> : IElementReference
        where T : Element
    {
        bool TryResolve(out T element);
    }
}
