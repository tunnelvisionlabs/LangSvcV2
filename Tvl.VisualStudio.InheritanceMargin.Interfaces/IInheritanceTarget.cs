namespace Tvl.VisualStudio.InheritanceMargin
{
    public interface IInheritanceTarget
    {
        string DisplayName
        {
            get;
        }

        void NavigateTo();
    }
}
