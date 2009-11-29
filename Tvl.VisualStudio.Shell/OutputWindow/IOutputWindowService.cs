namespace JavaLanguageService.Panes
{
    public interface IOutputWindowService
    {
        IOutputWindowPane TryGetPane(string name);
    }
}
