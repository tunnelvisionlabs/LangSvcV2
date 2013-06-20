namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    internal interface IElementReference<T> : IElementReference
        where T : Element
    {
        bool TryResolve(out T element);
    }
}
