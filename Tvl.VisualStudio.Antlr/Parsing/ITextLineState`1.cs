namespace Tvl.VisualStudio.Language.Parsing
{
    public interface ITextLineState<T>
        where T : struct, ITextLineState<T>
    {
        bool IsDirty
        {
            get;
        }

        bool IsMultiline
        {
            get;
        }

        T CreateDirtyState();

        T CreateMultilineState();
    }
}
