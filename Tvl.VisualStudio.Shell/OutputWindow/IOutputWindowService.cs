namespace Tvl.VisualStudio.Shell.OutputWindow
{
    public interface IOutputWindowService
    {
        IOutputWindowPane TryGetPane(string name);
    }
}
