namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    internal interface IElementReference
    {
        bool TryResolve(out Element element);
    }
}
